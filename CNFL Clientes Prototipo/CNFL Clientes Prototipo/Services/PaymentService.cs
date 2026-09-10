using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CNFL_Clientes_Prototipo.Data;
using CNFL_Clientes_Prototipo.Models;
using Newtonsoft.Json;

namespace CNFL_Clientes_Prototipo.Services
{
    /// <summary>
    /// Servicio centralizado para los 4 métodos de pago:
    ///   1. Tarjeta      -> Tilopay (redirect + callback)
    ///   2. SINPE Móvil  -> API BAC/BN (push al teléfono del cliente)
    ///   3. IBAN         -> Transferencia directa + subida de comprobante
    ///   4. Tokens       -> (Mock) pendiente integración con banco
    ///
    /// Configuración en Web.config:
    ///   - Tilopay:ApiKey, Tilopay:ApiUser, Tilopay:ApiPassword, Tilopay:Sandbox, Tilopay:BaseUrl
    ///   - SinpeMovil:Numero, SinpeMovil:Titular, SinpeMovil:ApiEndpoint, SinpeMovil:ApiKey
    ///   - Iban:Numero, Iban:Banco, Iban:Titular
    ///   - Tokens:ApiEndpoint, Tokens:ApiKey
    /// </summary>
    public class PaymentService
    {
        private readonly CNFLDbContext _db;

        public PaymentService()
        {
            _db = new CNFLDbContext();
        }

        public PaymentService(CNFLDbContext db)
        {
            _db = db;
        }

        // ================================================================
        // 1. TARJETA — TILOPAY
        // ================================================================
        /// <summary>
        /// Crea una orden de pago en Tilopay y devuelve la URL del checkout
        /// a la que se debe redirigir al cliente.
        /// </summary>
        public async Task<PagoResultado> CrearPagoTarjetaAsync(
            int facturaId,
            int usuarioId,
            decimal monto,
            string descripcion,
            string urlRetorno,
            string urlCallback)
        {
            var resultado = new PagoResultado { Metodo = "Tarjeta" };

            try
            {
                var apiKey = ConfigurationManager.AppSettings["Tilopay:ApiKey"];
                var apiUser = ConfigurationManager.AppSettings["Tilopay:ApiUser"];
                var apiPass = ConfigurationManager.AppSettings["Tilopay:ApiPassword"];
                var sandbox = bool.Parse(ConfigurationManager.AppSettings["Tilopay:Sandbox"] ?? "true");
                var baseUrl = ConfigurationManager.AppSettings["Tilopay:BaseUrl"]
                              ?? (sandbox ? "https://sandbox.tilopay.com" : "https://app.tilopay.com");

                if (string.IsNullOrWhiteSpace(apiKey) || apiKey.Contains("PON_TU"))
                {
                    // ⚠️ Sin credenciales reales: modo simulado para prototipo
                    var pagoSimulado = RegistrarPago(
                        facturaId, usuarioId, monto, "Tarjeta",
                        "SIMULADO-" + Guid.NewGuid().ToString("N").Substring(0, 8),
                        "Pendiente",
                        null);

                    resultado.Exito = true;
                    resultado.PagoId = pagoSimulado.PagoId;
                    resultado.ReferenciaExterna = pagoSimulado.ReferenciaExterna;
                    resultado.UrlRedireccion = urlRetorno + "?simulado=1&ref=" + pagoSimulado.ReferenciaExterna;
                    resultado.Mensaje = "Pago simulado (sin credenciales Tilopay). Redirigiendo...";
                    return resultado;
                }

                // ⚠️ Flujo real de Tilopay (ajustar según documentación oficial)
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

                    var payload = new
                    {
                        amount = monto,
                        currency = "CRC",
                        description = descripcion,
                        orderNumber = "CNFL-" + facturaId + "-" + DateTime.Now.Ticks,
                        returnUrl = urlRetorno,
                        callbackUrl = urlCallback,
                        customer = new { id = usuarioId }
                    };

                    var json = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var resp = await http.PostAsync($"{baseUrl}/api/v1/payment", content);
                    var body = await resp.Content.ReadAsStringAsync();

                    if (!resp.IsSuccessStatusCode)
                    {
                        resultado.Exito = false;
                        resultado.Mensaje = $"Tilopay {resp.StatusCode}: {body}";
                        return resultado;
                    }

                    dynamic data = JsonConvert.DeserializeObject(body);
                    var checkoutUrl = (string)data.checkoutUrl;
                    var referencia = (string)data.transactionId;

                    var pago = RegistrarPago(facturaId, usuarioId, monto, "Tarjeta", referencia, "Pendiente", body);

                    resultado.Exito = true;
                    resultado.PagoId = pago.PagoId;
                    resultado.ReferenciaExterna = referencia;
                    resultado.UrlRedireccion = checkoutUrl;
                    resultado.Mensaje = "Redirigiendo a Tilopay...";
                    return resultado;
                }
            }
            catch (Exception ex)
            {
                resultado.Exito = false;
                resultado.Mensaje = "Error: " + ex.Message;
                return resultado;
            }
        }

        // ================================================================
        // 2. SINPE MÓVIL
        // ================================================================
        /// <summary>
        /// Envía una solicitud de cobro SINPE Móvil al teléfono del cliente.
        /// El cliente recibe una notificación de su banco para aprobar el pago.
        /// </summary>
        public async Task<PagoResultado> CrearPagoSinpeAsync(
            int facturaId,
            int usuarioId,
            decimal monto,
            string telefonoCliente)
        {
            var resultado = new PagoResultado { Metodo = "SINPE Móvil" };

            try
            {
                var numeroCnfl = ConfigurationManager.AppSettings["SinpeMovil:Numero"] ?? "8000-0000";
                var titular = ConfigurationManager.AppSettings["SinpeMovil:Titular"] ?? "Compañía Nacional de Fuerza y Luz";
                var endpoint = ConfigurationManager.AppSettings["SinpeMovil:ApiEndpoint"];
                var apiKey = ConfigurationManager.AppSettings["SinpeMovil:ApiKey"];

                var referencia = "SINPE-" + facturaId + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");

                if (string.IsNullOrWhiteSpace(endpoint) || endpoint.Contains("PON_TU"))
                {
                    // ⚠️ Sin API de banco configurada: modo manual (el cliente paga al número)
                    var pagoSimulado = RegistrarPago(
                        facturaId, usuarioId, monto, "SINPE Móvil",
                        referencia, "Pendiente",
                        $"{{\"numero\":\"{numeroCnfl}\",\"titular\":\"{titular}\"}}");

                    resultado.Exito = true;
                    resultado.PagoId = pagoSimulado.PagoId;
                    resultado.ReferenciaExterna = referencia;
                    resultado.InstruccionesPago =
                        $"Realice el SINPE Móvil al número {numeroCnfl} ({titular}) " +
                        $"por ₡{monto:N0}. Referencia: {referencia}. " +
                        $"Luego suba el comprobante en la app.";
                    resultado.Mensaje = "Solicitud SINPE generada.";
                    return resultado;
                }

                // ⚠️ Flujo real con API del banco (BAC / BN / BCR)
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

                    var payload = new
                    {
                        destinationPhone = telefonoCliente,
                        amount = monto,
                        currency = "CRC",
                        reference = referencia,
                        concept = "Pago factura CNFL #" + facturaId
                    };

                    var json = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var resp = await http.PostAsync(endpoint, content);
                    var body = await resp.Content.ReadAsStringAsync();

                    if (!resp.IsSuccessStatusCode)
                    {
                        resultado.Exito = false;
                        resultado.Mensaje = $"SINPE {resp.StatusCode}: {body}";
                        return resultado;
                    }

                    dynamic data = JsonConvert.DeserializeObject(body);
                    var transactionId = (string)data.transactionId;

                    var pago = RegistrarPago(facturaId, usuarioId, monto, "SINPE Móvil", transactionId, "Pendiente", body);

                    resultado.Exito = true;
                    resultado.PagoId = pago.PagoId;
                    resultado.ReferenciaExterna = transactionId;
                    resultado.InstruccionesPago =
                        "Aprobé el pago desde la notificación de su banco. " +
                        "El sistema confirmará automáticamente cuando se reciba.";
                    resultado.Mensaje = "Solicitud SINPE enviada al celular.";
                    return resultado;
                }
            }
            catch (Exception ex)
            {
                resultado.Exito = false;
                resultado.Mensaje = "Error: " + ex.Message;
                return resultado;
            }
        }

        // ================================================================
        // 3. IBAN — TRANSFERENCIA
        // ================================================================
        /// <summary>
        /// Genera la referencia de transferencia IBAN.
        /// El cliente hace la transferencia desde su banco y sube el comprobante.
        /// </summary>
        public async Task<PagoResultado> CrearPagoIbanAsync(
            int facturaId,
            int usuarioId,
            decimal monto)
        {
            var resultado = new PagoResultado { Metodo = "IBAN" };

            try
            {
                var iban = ConfigurationManager.AppSettings["Iban:Numero"] ?? "CR00000000000000000000";
                var banco = ConfigurationManager.AppSettings["Iban:Banco"] ?? "Banco Nacional de Costa Rica";
                var titular = ConfigurationManager.AppSettings["Iban:Titular"] ?? "Compañía Nacional de Fuerza y Luz";

                var referencia = "IBAN-" + facturaId + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");

                var pago = RegistrarPago(
                    facturaId, usuarioId, monto, "IBAN",
                    referencia, "Pendiente",
                    $"{{\"iban\":\"{iban}\",\"banco\":\"{banco}\"}}");

                resultado.Exito = true;
                resultado.PagoId = pago.PagoId;
                resultado.ReferenciaExterna = referencia;
                resultado.InstruccionesPago =
                    $"Transfiera ₡{monto:N0} a la cuenta IBAN:\n\n" +
                    $"  IBAN: {iban}\n" +
                    $"  Banco: {banco}\n" +
                    $"  Titular: {titular}\n" +
                    $"  Referencia: {referencia}\n\n" +
                    $"Luego suba el comprobante en la app.";
                resultado.Mensaje = "Datos de transferencia IBAN generados.";
                return resultado;
            }
            catch (Exception ex)
            {
                resultado.Exito = false;
                resultado.Mensaje = "Error: " + ex.Message;
                return resultado;
            }
        }

        // ================================================================
        // 4. TOKENS — BANCO (MOCK)
        // ================================================================
        /// <summary>
        /// Genera una solicitud de pago con token bancario.
        /// ⚠️ TODO: integración con API del banco emisor del token.
        /// </summary>
        public async Task<PagoResultado> CrearPagoTokenAsync(
            int facturaId,
            int usuarioId,
            decimal monto,
            string bancoEmisor)
        {
            var resultado = new PagoResultado { Metodo = "Tokens" };

            try
            {
                var endpoint = ConfigurationManager.AppSettings["Tokens:ApiEndpoint"];
                var apiKey = ConfigurationManager.AppSettings["Tokens:ApiKey"];
                var referencia = "TOKEN-" + facturaId + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");

                if (string.IsNullOrWhiteSpace(endpoint) || endpoint.Contains("PON_TU"))
                {
                    // ⚠️ Sin API configurada: mock
                    var pagoSimulado = RegistrarPago(
                        facturaId, usuarioId, monto, "Tokens",
                        referencia, "Pendiente",
                        $"{{\"banco\":\"{bancoEmisor}\",\"mock\":true}}");

                    resultado.Exito = true;
                    resultado.PagoId = pagoSimulado.PagoId;
                    resultado.ReferenciaExterna = referencia;
                    resultado.InstruccionesPago =
                        $"Su banco emisor ({bancoEmisor}) le enviará un token a su dispositivo.\n" +
                        $"Ingrese el token de 6 dígitos en el siguiente paso.";
                    resultado.Mensaje = "Esperando token del banco.";
                    return resultado;
                }

                // ⚠️ Flujo real (ajustar según banco)
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

                    var payload = new { amount = monto, reference = referencia, bank = bancoEmisor };
                    var json = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var resp = await http.PostAsync(endpoint, content);
                    var body = await resp.Content.ReadAsStringAsync();

                    if (!resp.IsSuccessStatusCode)
                    {
                        resultado.Exito = false;
                        resultado.Mensaje = $"Tokens {resp.StatusCode}: {body}";
                        return resultado;
                    }

                    var pago = RegistrarPago(facturaId, usuarioId, monto, "Tokens", referencia, "Pendiente", body);

                    resultado.Exito = true;
                    resultado.PagoId = pago.PagoId;
                    resultado.ReferenciaExterna = referencia;
                    resultado.Mensaje = "Solicitud de token enviada.";
                    return resultado;
                }
            }
            catch (Exception ex)
            {
                resultado.Exito = false;
                resultado.Mensaje = "Error: " + ex.Message;
                return resultado;
            }
        }

        // ================================================================
        // HELPERS
        // ================================================================
        private Pago RegistrarPago(
            int facturaId, int usuarioId, decimal monto,
            string metodo, string referencia, string estado, string rawResponse)
        {
            var pago = new Pago
            {
                FacturaId = facturaId,
                UsuarioId = usuarioId,
                Monto = monto,
                Metodo = metodo,
                ReferenciaExterna = referencia,
                Estado = estado,
                FechaCreacion = DateTime.Now,
                RawResponse = rawResponse
            };
            _db.Pagos.Add(pago);
            _db.SaveChanges();
            return pago;
        }

        /// <summary>
        /// Marca un pago como confirmado (se llama desde el callback/webhook).
        /// </summary>
        public async Task<bool> ConfirmarPagoAsync(string referenciaExterna)
        {
            var pago = await _db.Pagos.FirstOrDefaultAsync(p => p.ReferenciaExterna == referenciaExterna);
            if (pago == null) return false;

            pago.Estado = "Confirmado";
            pago.FechaConfirmacion = DateTime.Now;

            // Marcar la factura como pagada
            var factura = await _db.Facturas.FindAsync(pago.FacturaId);
            if (factura != null)
            {
                factura.Pagada = true;
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }

    // ===== RESULTADO =====
    public class PagoResultado
    {
        public bool Exito { get; set; }
        public int PagoId { get; set; }
        public string Metodo { get; set; }
        public string ReferenciaExterna { get; set; }
        public string UrlRedireccion { get; set; }
        public string InstruccionesPago { get; set; }
        public string Mensaje { get; set; }
        public List<string> Errores { get; set; } = new List<string>();
    }
}
# Validaciones del expediente, permisos y vacaciones

## Reglas reforzadas

- Las decisiones de vacaciones requieren `aprobar: true` o `aprobar: false` explícito.
  Rechazar requiere una observación; la aprobación conserva observación opcional.
- Fechas de vacaciones omitidas o anteriores a 1900 se rechazan. Las solicitudes
  nuevas no admiten inicio pasado. RRHH conserva el registro de descuentos históricos.
- Descuentos, ajustes, anulaciones, aprobaciones y cambios de días asignados se
  coordinan mediante transacciones y bloqueos PostgreSQL. No se permiten anulaciones
  de ajustes ni reducciones de días asignados que produzcan saldo negativo.
- Los ajustes acreditan entre 1 y 365 días enteros. Los descuentos se registran por
  rango de fechas; no se usan ajustes negativos.
- Cargo y área activos; ciudad, provincia y región activas; género, etnia,
  estado civil, tipo de sangre y bancos existentes y activos.
- Edición individual de jefe sin autoasignaciones ni ciclos; el destino debe
  estar activo y marcado como jefe.
- El expediente completo requiere cargo, fecha de ingreso válida, al menos una
  cuenta bancaria y un contacto de emergencia. Se valida el resultado después
  de altas, ediciones y bajas. Un expediente antiguo incompleto debe corregirse
  antes de guardar nuevas modificaciones.
- Nombres entre 2 y 50 caracteres, con letras Unicode y separadores habituales.
  Celulares opcionales: 10 dígitos comenzando con 09. Contactos de emergencia:
  teléfono obligatorio de 7 a 10 dígitos. Se conserva el formato local actual.
- Nacimiento no futuro y no anterior a 1900; ingreso posterior al nacimiento.
  Hijos requieren nacimiento no futuro; cónyuge requiere unión no futura y
  posterior al nacimiento del empleado. Títulos con fecha informada: no futura
  y posterior al nacimiento. No se impone una edad mínima laboral adicional.
- Cuentas de AHORROS o CORRIENTE y números de 3 a 30 dígitos.
- Cédula y correos con índices únicos, incluidos los usuarios inactivos. La
  aplicación normaliza correos a minúsculas y recorta espacios. Los conflictos
  simultáneos devuelven 409, sin detalles internos de base de datos.
- No se permite autodesactivar la cuenta ni retirar el último acceso activo
  ADMIN/RRHH. Sus nombres y estados de catálogo se protegen como roles del sistema.
  Cambiar/desactivar otros roles revoca las sesiones de sus usuarios.
- No se bloquea la desactivación de un jefe por tener equipo o solicitudes:
  RRHH puede usar el traspaso de equipo para reasignarlos.
- Cambiar o recuperar contraseña también incrementa la versión de acceso.
  Una contraseña nueva debe diferir de la actual; límite de 72 bytes UTF-8 por
  BCrypt. Los espacios y las letras acentuadas no cuentan como símbolo especial.
- Fotos y banners: hasta 5 MB, MIME JPG/PNG/WEBP coherente con la firma binaria.
  Cloudinary realiza la decodificación de imagen antes de guardar la URL.
  El límite HTTP es 6 MB para permitir la envoltura multipart.

## Límite de intentos

Por IP de conexión y por proceso de aplicación, sin cola:

| Operación | Intentos | Ventana |
|---|---:|---|
| Login | 10 | 1 minuto |
| Cambio de contraseña | 5 | 5 minutos |
| Recuperación | 3 | 15 minutos |

Se devuelve 429 con mensaje y `Retry-After`. En instalaciones con proxy inverso,
configurar explícitamente los proxies confiables antes de usar la IP reenviada;
no se confía automáticamente en cualquier `X-Forwarded-For`. Con varias instancias,
los contadores son independientes. No se implementa bloqueo permanente de cuentas.

## Verificación

Desde la carpeta `Intranet`:

```powershell
dotnet run --project tests/Validaciones/Validaciones.csproj -c Release -p:UseAppHost=false
dotnet tests/Validaciones/bin/Release/net10.0/Validaciones.dll --database
```

La segunda opción usa la conexión configurada, crea un esquema temporal
`validaciones_<guid>`, carga solo datos ficticios, usa un correo simulado y elimina
ese esquema en `finally`. No modifica colaboradores de la aplicación. Necesita
permiso para crear esquemas y al menos dos conexiones simultáneas disponibles.

Desde el frontend:

```powershell
node --test tests/validacionesUsuario.test.js
npm run build
```

La migración `UnicidadColaboradores` crea los índices únicos. Si existen duplicados
previos, falla sin eliminarlos: deben revisarse antes de aplicarla.

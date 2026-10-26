# Equipos y vacaciones

Todos los endpoints requieren JWT. No se necesita migración de base de datos.

## RRHH: reemplazar jefe

`GET /api/Usuarios/jefes` devuelve únicamente personas marcadas `EsJefe`,
con cargo, área, estado y cantidad de empleados directos. Los inactivos se
incluyen para poder reemplazarlos; el selector de destino debe filtrar `estado: true`.
Incluye `cantidadSolicitudesPendientes` para la confirmación previa al traspaso.

`GET /api/Usuarios/jefes/{idJefe}/empleados` permite exclusivamente a RRHH
consultar el equipo directo de un jefe seleccionado, incluidos los empleados
inactivos. Devuelve 404 si la persona no existe o no está marcada como jefe.

`POST /api/Usuarios/traspasar-equipo`

```json
{ "idJefeAnterior": 10, "idJefeNuevo": 20 }
```

La respuesta incluye `idJefeAnterior`, `idJefeNuevo`, `empleadosTrasladados`
y `solicitudesPendientesTrasladadas`. Los IDs inválidos, jefes iguales,
destino inactivo, origen sin equipo y ciclos devuelven 400; otros roles, 403.

Se trasladan todos los empleados directos, incluidos los inactivos, sin filtrar
por área. No se cambia la marca `EsJefe` del anterior. Solo se reasignan las
solicitudes pendientes del equipo; las resueltas conservan su aprobador.

Si el destino es integrante del equipo, hereda el superior del jefe anterior.
Sus propias solicitudes pendientes se envían a ese superior, que debe ser un
jefe activo; si no existe, se rechaza toda la operación sin cambios parciales.
El contador de empleados incluye al nuevo jefe cuando cambia su superior.

El traspaso y la creación/resolución de solicitudes se coordinan con bloqueos
de tablas PostgreSQL dentro de transacciones breves. Esto serializa esas
escrituras y mantiene las lecturas normales disponibles. Los correos de
aprobación se envían después de confirmar la transacción.

## Jefes: mis empleados

`GET /api/Usuarios/mis-empleados` toma la identidad exclusivamente del JWT.
Exige que el usuario esté activo y marcado como jefe; devuelve 403 en otro caso.
Incluye empleados directos activos e inactivos y devuelve solamente ID, nombre,
apellido, estado, beneficio de vacaciones, cargo y área. No expone datos
bancarios, familiares ni el perfil personal completo.

Para el empleado seleccionado se reutilizan:

- `GET /api/Vacaciones/saldo/{idUsuario}`
- `GET /api/Vacaciones/historial/{idUsuario}`

Además del propio empleado y RRHH, puede consultar su jefe directo actual,
si está activo y marcado como jefe. Cambiar el ID por alguien ajeno devuelve 403.
No se amplían permisos de edición ni del perfil completo. Los permisos existentes
de descarga de documentos y el listado de solicitudes pendientes no se modifican.

## Comprobaciones de integración pendientes

Usar una base de pruebas, nunca datos reales, para verificar:

1. Empleado normal: listado de jefes y traspaso devuelven 403.
2. Jefe: solo recibe su equipo; saldo e historial ajenos devuelven 403.
3. RRHH: traspasa un equipo y sus solicitudes pendientes; las resueltas no cambian.
4. Tras el traspaso, el jefe anterior pierde acceso al saldo e historial del equipo.
5. Promover un integrante evita autoasignación y deriva sus pendientes al superior.
6. Destino inactivo, jefe idéntico y jerarquía cíclica fallan sin cambios parciales.
7. Crear o resolver una solicitud mientras se traspasa no deja un aprobador obsoleto.

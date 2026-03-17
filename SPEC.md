# IAM Platform - Requisitos Iniciales

## Objetivo del documento
- Capturar requisitos minimos para guiar una implementacion asistida por IA.
- Mantener el alcance pequeno y refinable.
- No incluir planificacion, roadmap ni decisiones tecnicas cerradas.

## Estado actual
- Nombre tentativo: IAM Platform
- Etapa: descubrimiento
- Version de spec: v0.6

## Vision del producto
Una aplicacion web centralizada de IAM que permita administrar identidades y autorizacion para multiples tenants y sus aplicaciones, ofreciendo autenticacion central, consulta de decisiones efectivas de acceso y registro de eventos de seguridad.

## Problema
Hoy la seguridad suele resolverse por aplicacion, duplicando usuarios, roles, reglas de acceso y logs. Esto genera inconsistencia, poca trazabilidad y mayor costo operativo. El producto busca centralizar estas capacidades en un solo sistema.

## Actores iniciales
- Usuario de sistema: identidad interna del servicio; administra la plataforma global, no pertenece a ningun tenant y puede invitar otros usuarios de sistema.
- Administrador de tenant: identidad de cliente; pertenece a un tenant, puede registrarse creando el tenant inicial, administra su configuracion con alcance total sobre su tenant y puede invitar otros administradores de tenant.
- Usuario final de tenant: identidad perteneciente a un tenant; es el sujeto principal evaluado por las reglas de acceso de las aplicaciones integradas.
- Usuario de servicio administrativo: identidad no humana perteneciente a un tenant; administra configuracion por API de forma automatizada.
- Aplicacion consumidora: delega autenticacion y consulta decisiones efectivas de acceso.

## Modelo de dominio inicial
- Tenant: limite principal de aislamiento funcional.
- Usuario de sistema: identidad global administrativa fuera del contexto de tenant, con alcance global sobre la plataforma.
- Usuario de tenant: identidad autenticable perteneciente exactamente a un tenant; puede ser administrador de tenant, usuario final o usuario de servicio administrativo.
- Rol: pertenece exactamente a un tenant.
- Aplicacion: pertenece exactamente a un tenant.
- Recurso: identificador string de un objeto de seguridad dentro de una aplicacion.
- Operacion: identificador string de una accion posible sobre un recurso.
- Arbol de recursos: jerarquia de recursos dentro de una aplicacion.
- Regla de autorizacion: tupla `(usuarioTenant?, rol?, recurso, operacion, decision)`.
- Decision de autorizacion: valor `permitido`, `denegado` o `heredado`.
- Evento de auditoria: registro de acciones de autenticacion, autorizacion y administracion.

## Reglas estructurales del core
- El sistema debe soportar multiples tenants.
- El sistema debe soportar usuarios de sistema globales y usuarios de tenant.
- Debe existir un unico usuario de sistema inicial creado al bootstrap de la plataforma.
- Un usuario de sistema no pertenece a ningun tenant.
- Un usuario de sistema puede invitar otros usuarios de sistema.
- Un usuario de tenant pertenece a un unico tenant y no puede existir en varios tenants al mismo tiempo.
- Un rol pertenece a un unico tenant y no puede existir en varios tenants al mismo tiempo.
- Una aplicacion pertenece a un unico tenant y no puede existir en varios tenants al mismo tiempo.
- Un tenant puede tener multiples usuarios de tenant, roles y aplicaciones.
- El registro inicial de un administrador de tenant debe poder crear un tenant nuevo.
- Un administrador de tenant tiene acceso administrativo completo sobre su tenant por tipo de usuario, sin requerir roles ni reglas de autorizacion para ese alcance administrativo.
- Un administrador de tenant puede invitar otros administradores de tenant dentro de su mismo tenant.
- Cada aplicacion debe tener un arbol de recursos.
- Cada recurso debe poder ubicarse dentro de un unico arbol de una aplicacion.
- Cada recurso debe poder definir una o mas operaciones.
- Cada operacion debe identificarse por un string dentro del contexto de su recurso.
- Un usuario de tenant puede tener multiples roles dentro de su tenant.
- Una regla de autorizacion debe referenciar al menos un sujeto: `usuarioTenant`, `rol` o ambos.
- Una regla de autorizacion nunca puede tener `usuarioTenant` y `rol` vacios al mismo tiempo.
- `UsuarioTenant` y `rol` son opcionales individualmente, pero no pueden ser ambos nulos en la misma regla.
- Una regla de autorizacion debe referenciar exactamente un `recurso` y una `operacion` de ese recurso.
- La decision de una regla de autorizacion solo puede ser `permitido`, `denegado` o `heredado`.
- Las reglas de autorizacion del dominio tenant solo pueden referenciar usuarios de tenant, nunca usuarios de sistema.

## Modelo de autorizacion inicial
- El contexto minimo de evaluacion es `tenant + aplicacion + usuarioTenant + recurso + operacion`.
- Las reglas aplicables a una evaluacion son las que coinciden con el recurso y la operacion consultados, y con el usuario de tenant o los roles asignados a ese usuario.
- Pueden existir multiples reglas aplicables, incluso redundantes o contradictorias.
- Si una regla tiene `usuarioTenant = null`, aplica solo por coincidencia de rol.
- Si una regla tiene `rol = null`, aplica solo por coincidencia de usuario de tenant.
- Si una regla tiene `usuarioTenant` y `rol` informados, aplica solo cuando ambos coinciden con la evaluacion.
- Si la decision de una regla es `heredado`, el valor debe resolverse buscando la misma `operacion` en el recurso padre dentro del arbol.
- La evaluacion de herencia debe poder repetirse hacia arriba hasta encontrar un valor explicito para la misma `operacion` o llegar a la raiz del arbol.
- Si al menos una regla aplicable resuelve en `denegado`, la decision efectiva es `denegado`.
- La decision efectiva es `permitido` solo cuando todas las reglas aplicables resueltas terminan en `permitido`.
- Si no existe ninguna regla aplicable para el recurso y la operacion consultados, la decision efectiva es `denegado`.
- Si una cadena de herencia llega a la raiz sin encontrar un valor explicito para la misma `operacion`, ese resultado se ignora y no aporta una decision resuelta.
- Si despues de ignorar resultados no resueltos por herencia no queda ninguna regla resuelta aplicable, la decision efectiva es `denegado`.
- Las decisiones efectivas del dominio tenant se evaluan sobre usuarios de tenant, no sobre usuarios de sistema.
- El acceso administrativo global de usuarios de sistema queda fuera del modelo de reglas del tenant.
- El acceso administrativo de un administrador de tenant sobre su tenant queda fuera del modelo de reglas de recursos y se concede implicitamente por tipo de usuario.

## Alcance funcional inicial
- Administrar tenants, usuarios de sistema, usuarios de tenant, roles, aplicaciones y recursos desde una UI web.
- Permitir el bootstrap de un unico usuario de sistema inicial y la invitacion de otros usuarios de sistema.
- Permitir el registro de un administrador de tenant creando el tenant inicial.
- Permitir que un administrador de tenant invite otros administradores de tenant.
- Administrar operaciones definidas sobre cada recurso.
- Asignar roles a usuarios de tenant dentro de un tenant.
- Administrar reglas de autorizacion sobre recursos y operaciones.
- Permitir que otras aplicaciones usen autenticacion central.
- Permitir que otras aplicaciones consulten decisiones efectivas de acceso.
- Permitir administracion automatizada por API mediante usuarios de servicio administrativo.
- Guardar logs de autenticacion, autorizacion y cambios administrativos.

## Requisitos funcionales
- RF-01: El sistema debe soportar multiples tenants.
- RF-02: El sistema debe permitir crear, editar, activar y desactivar tenants.
- RF-03: El sistema debe inicializarse con un unico usuario de sistema bootstrap.
- RF-04: El sistema debe permitir crear, editar, activar y desactivar usuarios de sistema.
- RF-05: Un usuario de sistema no debe pertenecer a ningun tenant.
- RF-06: Un usuario de sistema autenticado debe poder invitar otro usuario de sistema.
- RF-07: El sistema debe permitir el registro de un administrador de tenant nuevo.
- RF-08: Cuando un administrador de tenant se registra por primera vez, el sistema debe crear un tenant nuevo y asociarlo a ese administrador.
- RF-09: El sistema debe permitir crear, editar, activar y desactivar usuarios de tenant.
- RF-10: El sistema debe distinguir al menos estos tipos de usuario de tenant: administrador de tenant, usuario final y usuario de servicio administrativo.
- RF-11: Cada usuario de tenant debe pertenecer exactamente a un tenant.
- RF-12: Un administrador de tenant debe tener acceso administrativo completo sobre su tenant sin requerir roles ni reglas de autorizacion para ese alcance administrativo.
- RF-13: Un administrador de tenant autenticado debe poder invitar otros administradores de tenant dentro de su tenant.
- RF-14: El sistema debe permitir crear, editar y desactivar roles.
- RF-15: El sistema debe permitir asignar multiples roles a un usuario de tenant dentro de su tenant.
- RF-16: El sistema debe permitir registrar aplicaciones dentro de un tenant.
- RF-17: Cada aplicacion debe poder definir y mantener un arbol de recursos.
- RF-18: Cada recurso debe identificarse por un string dentro del contexto de su aplicacion.
- RF-19: Cada recurso debe permitir crear, editar y desactivar operaciones.
- RF-20: El sistema debe permitir crear, editar y eliminar reglas de autorizacion asociadas a un recurso y una operacion concreta.
- RF-21: Cada regla de autorizacion debe referenciar al menos un `usuarioTenant` o un `rol`.
- RF-22: Cada regla de autorizacion debe aceptar solo las decisiones `permitido`, `denegado` o `heredado`.
- RF-23: Las reglas de autorizacion del dominio tenant no deben poder referenciar usuarios de sistema.
- RF-24: El sistema debe poder resolver herencia de reglas siguiendo el padre del recurso para la misma operacion dentro del arbol de la aplicacion.
- RF-25: El sistema debe poder evaluar la decision efectiva de autorizacion para un usuario de tenant sobre un recurso y una operacion, considerando el usuario y todos sus roles asignados.
- RF-26: El sistema debe soportar multiples reglas aplicables sobre un mismo contexto, incluso si son redundantes o contradictorias.
- RF-27: Si existe al menos una regla aplicable que resuelve en `denegado`, la decision efectiva debe ser `denegado`.
- RF-28: Si todas las reglas aplicables resueltas terminan en `permitido`, la decision efectiva debe ser `permitido`.
- RF-28a: Si no existe ninguna regla aplicable para un recurso y una operacion, la decision efectiva debe ser `denegado`.
- RF-28b: Si una cadena de herencia llega a la raiz sin encontrar un valor explicito para la misma operacion, esa regla no debe aportar una decision resuelta.
- RF-28c: Si, despues de ignorar reglas no resueltas por herencia, no queda ninguna regla resuelta aplicable, la decision efectiva debe ser `denegado`.
- RF-29: El sistema debe exponer una capacidad de autenticacion central para aplicaciones integradas.
- RF-30: El sistema debe permitir a una aplicacion consultar la decision efectiva de acceso de un usuario de tenant para un recurso y una operacion concretos.
- RF-31: El sistema debe permitir que un usuario de servicio administrativo autenticado ejecute acciones de administracion por API dentro de su tenant sin requerir sesion interactiva de un administrador de tenant.
- RF-32: El sistema debe registrar eventos de login, consultas de autorizacion, invitaciones y cambios administrativos.
- RF-33: Cada evento de auditoria debe guardar como minimo actor, tipo de actor, tenant cuando aplique, aplicacion cuando aplique, accion, objetivo, resultado y fecha/hora.
- RF-34: La UI administrativa debe permitir visualizar tenants, usuarios de sistema, usuarios de tenant, roles, aplicaciones, recursos, operaciones, reglas de autorizacion e invitaciones y logs.

## Requisitos no funcionales iniciales
- RNF-01: La informacion de acceso debe ser consistente entre la UI administrativa y las consultas hechas por aplicaciones.
- RNF-02: Los cambios de seguridad deben quedar auditables.
- RNF-03: El sistema debe estar pensado para multiples tenants y multiples aplicaciones por tenant.
- RNF-04: El modelo de recursos debe soportar jerarquia sin perder trazabilidad de la decision efectiva.
- RNF-05: El modelo debe permitir evaluar reglas por operacion sin ambiguedad.
- RNF-06: El sistema debe mantener aislamiento claro entre el alcance global de usuarios de sistema y el alcance de cada tenant.
- RNF-07: La spec debe poder refinarse sin reescribirse completa cada vez.

## Criterios de aceptacion iniciales
- Dado el bootstrap inicial de la plataforma, cuando el sistema queda operativo por primera vez, entonces existe exactamente un usuario de sistema inicial.
- Dado un usuario de sistema autenticado, cuando invita otro usuario de sistema, entonces la identidad invitada puede incorporarse como usuario de sistema sin quedar asociada a ningun tenant.
- Dado un visitante, cuando se registra como administrador de tenant, entonces el sistema crea un tenant nuevo y lo asocia a ese administrador.
- Dado un administrador de tenant autenticado, cuando administra configuracion de su tenant, entonces el sistema permite la operacion sin requerir roles ni reglas de autorizacion adicionales.
- Dado un administrador de tenant autenticado, cuando invita otro administrador de tenant, entonces la identidad invitada queda asociada al mismo tenant con alcance administrativo.
- Dado un tenant con dos aplicaciones, cuando un administrador registra usuarios de tenant y roles, entonces todos quedan asociados al tenant correcto.
- Dado un usuario de servicio administrativo autenticado para un tenant, cuando invoca la API administrativa para actualizar configuracion del tenant, entonces el sistema procesa la operacion dentro de ese tenant y registra auditoria.
- Dado una aplicacion con un arbol de recursos, cuando un administrador registra un recurso hijo, entonces el sistema conserva la relacion con su recurso padre.
- Dado un recurso, cuando un administrador define operaciones como `read` o `write`, entonces esas operaciones quedan disponibles para asociar reglas de autorizacion sobre ese recurso.
- Dado un usuario de tenant con multiples roles y varias reglas aplicables sobre un recurso y una operacion, cuando todas resuelven en `permitido`, entonces la decision efectiva es positiva.
- Dado un usuario de tenant con multiples roles y al menos una regla aplicable que resuelve en `denegado`, cuando se consulta el acceso sobre un recurso y una operacion, entonces la decision efectiva es negativa.
- Dado dos reglas aplicables contradictorias sobre el mismo recurso y la misma operacion, cuando una resuelve en `permitido` y otra en `denegado`, entonces la decision efectiva es negativa.
- Dado una regla con decision `heredado` para la operacion `read` en un recurso hijo y una decision explicita para `read` en el padre, cuando se consulta la decision efectiva de `read`, entonces el valor del hijo se resuelve a partir del padre.
- Dado una regla con decision `heredado` para la operacion `read` en un recurso hijo y una decision explicita para `write` en el padre, cuando se consulta la decision efectiva de `read`, entonces el valor de `write` no debe intervenir en la resolucion.
- Dado un usuario de tenant y un recurso sin ninguna regla aplicable para la operacion consultada, cuando se evalua la decision efectiva, entonces el resultado es `denegado`.
- Dado una cadena de herencia para la operacion consultada que llega a la raiz sin encontrar un valor explicito y existe otra regla aplicable que resuelve en `permitido`, cuando se evalua la decision efectiva, entonces el resultado final es `permitido`.
- Dado una cadena de herencia para la operacion consultada que llega a la raiz sin encontrar un valor explicito y existe otra regla aplicable que resuelve en `denegado`, cuando se evalua la decision efectiva, entonces el resultado final es `denegado`.
- Dado una cadena de herencia para la operacion consultada que llega a la raiz sin encontrar un valor explicito y no existe ninguna otra regla resuelta aplicable, cuando se evalua la decision efectiva, entonces el resultado es `denegado`.
- Dado una regla con `usuarioTenant = null`, cuando el usuario evaluado posee el rol indicado, entonces la regla aplica por rol.
- Dado una regla con `rol = null`, cuando el usuario evaluado coincide con el usuario de la regla, entonces la regla aplica por usuario.
- Dado un cambio administrativo en tenants, usuarios, roles, recursos o reglas de autorizacion, cuando la operacion finaliza, entonces queda registrado un evento de auditoria.

## No objetivos por ahora
- MFA.
- Federacion con proveedores externos.
- ABAC o motor de politicas avanzado.
- Jerarquias organizacionales complejas.
- Autoservicio de usuarios finales.
- Cambios automaticos de estructura de seguridad desde aplicaciones externas.

## Supuestos y riesgos
- El primer alcance puede resolverse con un modelo RBAC extendido por reglas directas a usuarios de tenant.
- La coexistencia de reglas redundantes o contradictorias puede volver mas compleja la explicacion del resultado efectivo.
- La definicion de operaciones por recurso agrega flexibilidad, pero tambien aumenta el riesgo de inconsistencias semanticas entre aplicaciones.
- La convivencia entre usuarios de sistema globales y usuarios de tenant exige limites de alcance muy claros para evitar errores de aislamiento.
- Los privilegios implicitos de usuarios de sistema y administradores de tenant introducen un camino de autorizacion fuera del motor de reglas y deben mantenerse claramente separado.
- Ignorar herencias no resueltas evita falsos negativos sobre reglas explicitas concurrentes, pero aumenta la necesidad de explicar por que algunas reglas no aportaron decision.
- Las aplicaciones integradas pertenecen a un contexto de confianza comun.
- El detalle exacto del protocolo de SSO aun no esta decidido.

## Preguntas abiertas
- Los usuarios de servicio administrativo usan el mismo modelo de roles y reglas que los usuarios humanos del tenant o un esquema separado?
- Las invitaciones crean la identidad inmediatamente o requieren aceptacion posterior?
- Un administrador de tenant puede tambien actuar como usuario final sujeto a reglas de aplicacion, o su alcance debe mantenerse solo administrativo?
- El identificador string del recurso debe ser unico dentro de la aplicacion o unico por rama?
- Las operaciones deben ser libres por recurso o debe existir un catalogo comun por aplicacion o tenant?
- Los roles pertenecen al tenant completo o pueden quedar restringidos a una aplicacion?
- Deben existir operaciones de movimiento de recursos dentro del arbol y como afectan la herencia?
- El SSO se resolvera con OIDC, OAuth2, SAML u otra alternativa?
- Los logs deben ser solo consultables o tambien inmutables?
- Los recursos de seguridad se cargan manualmente o se sincronizan desde cada aplicacion?

## Cuando refinar esta spec
- Cuando cambie el modelo de autorizacion.
- Cuando se cierre una decision clave sobre herencia, default de decisiones o alcance de actores.
- Cuando se agregue una nueva capacidad de negocio relevante.
- Cuando un requisito deje de ser verificable con los criterios actuales.

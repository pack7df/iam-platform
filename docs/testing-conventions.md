# Testing Conventions

## Objetivo
- Mantener los tests faciles de leer, revisar y relacionar con el comportamiento esperado del producto.
- Mejorar la trazabilidad entre tests automatizados, reglas del dominio y requisitos documentados en `SPEC.md`.

## Convencion para nuevos archivos de tests
- Cada archivo nuevo de tests debe empezar con una cabecera breve en comentarios.
- La cabecera debe indicar:
  - que componente o servicio se esta validando;
  - que comportamiento principal se comprueba;
  - con que requisitos, reglas o lineas de `SPEC.md` se relaciona.

## Formato sugerido
```csharp
// Valida la precedencia y agregacion de decisiones resueltas del motor de autorizacion.
// Comprueba conflictos, redundancias y combinacion de reglas aplicables.
// Cubre: SPEC.md:76, SPEC.md:77, SPEC.md:79, SPEC.md:80
```

## Reglas de uso
- Usar cabeceras cortas, concretas y orientadas al comportamiento.
- Evitar comentarios redundantes dentro de cada test cuando el nombre del test ya sea suficientemente claro.
- Preferir nombres de test que expliquen el escenario y el resultado esperado.
- Cuando un archivo cubra multiples grupos de escenarios, se pueden agregar comentarios breves para separar bloques.
- Referenciar `SPEC.md` por linea o por requisito cuando exista una relacion clara y estable.

## Alcance inicial
- Esta convencion se aplica desde la Etapa 2 en adelante para los nuevos archivos de tests.
- No es obligatorio reescribir todos los tests previos de inmediato, pero cualquier refactor relevante puede aprovechar para alinearlos.

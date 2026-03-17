# Plantilla de Tarea y PR para IA

## Uso
- Completar esta plantilla antes de implementar una tarea.
- Una tarea debe producir una rama y un PR principal.
- Objetivo de tamano: hasta 600 lineas cambiadas.
- Limite maximo: 1000 lineas cambiadas.
- Si la tarea excede el limite, debe dividirse antes de implementarse.
- El flujo esperado es TDD: `red -> green -> refactor`.

## 1. Identificacion de la tarea
- Task ID:
- Tipo: `feat` | `fix` | `refactor` | `test` | `docs` | `chore`
- Titulo corto:
- Branch name: `<tipo>/<task-id>-<short-slug>`
- PR title: `[<task-id>] <tipo>: <resumen corto>`

## 2. Contexto
- Problema a resolver:
- Resultado esperado:
- Relacion con `SPEC.md`:
- Relacion con `TECH_SPEC.md`:

## 3. Alcance
- En alcance:
  - 
  - 
- Fuera de alcance:
  - 
  - 

## 4. Impacto tecnico
- Modulos afectados:
- Archivos o carpetas esperadas:
- Riesgos tecnicos:
- Dependencias o precondiciones:

## 5. Plan TDD
- `Red` - tests a escribir primero:
  - 
  - 
- `Green` - implementacion minima esperada:
  - 
  - 
- `Refactor` - limpieza posterior esperada:
  - 
  - 

## 6. Plan de tamano del PR
- Lineas estimadas:
- Estrategia para mantenerse bajo 600 lineas:
- Si supera 600, justificacion:
- Si podria superar 1000, plan de division:

## 7. Verificacion
- Tests a ejecutar:
  - 
  - 
- Validaciones manuales:
  - 
  - 

## 8. Criterios de terminado
- [ ] Tests agregados o actualizados.
- [ ] Implementacion minima completa.
- [ ] Refactor aplicado sin romper tests.
- [ ] PR dentro del tamano objetivo o con justificacion.
- [ ] Sin mezclar cambios conceptuales no relacionados.
- [ ] Documentacion actualizada si corresponde.

## 9. Plantilla de PR
```md
## Contexto
- Task ID: <task-id>
- Objetivo: <objetivo corto>

## Cambios
- <cambio 1>
- <cambio 2>

## Alcance
- Incluye: <scope>
- No incluye: <out of scope>

## Testing
- <test command 1>
- <test command 2>

## Tamano y revision
- Lineas cambiadas estimadas/reales: <n>
- Justificacion si supera 600 lineas: <texto>

## Riesgos o notas
- <riesgo o nota>
```

## 10. Regla operativa para IA
- No iniciar implementacion si la tarea no tiene alcance claro.
- No mezclar refactor, feature y cambios de infraestructura en el mismo PR salvo necesidad justificada.
- Preferir cambios pequenos, verticales y revisables.
- Si una tarea crece demasiado, detenerse y proponer division antes de seguir.

-- MV 28052026 (seguimiento): rendimiento de la paginación en la Maestra de Socios (MSocios).
--
-- Diagnóstico:
-- La vista de Socios ya usa paginación real server-side (Kendo Grid con ServerOperation(true),
-- MSociosController.ReadMSocios traduce el paginado a OFFSET/FETCH vía ToDataSourceResult).
-- El problema NO era de código: dbo.MSocios no tenía ningún índice sobre dFechaActualizacion,
-- que es la columna de orden por defecto del grid. Sin índice, SQL Server tiene que ordenar la
-- tabla completa en cada petición para poder aplicar el OFFSET/FETCH, lo que se vuelve muy lento
-- a medida que el usuario avanza de página (a más profundidad de página, peor).
--
-- Medido en local (CLUBS_AX_TEST, 30.208 filas en MSocios):
--   Página 1  (OFFSET 0)      sin índice: ~39 ms   | con índice: ~41 ms  (sin cambio notable)
--   Página profunda (OFFSET 20000) sin índice: ~25.131 ms | con índice: ~8 ms
--
-- Fix: un único índice no-clustered sobre la columna de orden. No cambia ninguna consulta,
-- no afecta a los filtros de texto (sCodSAP, sNIF, sNombreFcia, sDireccion, sCP, sPoblacion,
-- que usan LIKE '%valor%' y no pueden aprovechar un índice B-tree estándar de todos modos), y
-- no requiere cambios en el .edmx (un índice no se modela ahí).
--
-- Aplicar en cada entorno donde exista CLUBS_AX_TEST (local, TEST, DESARROLLO/producción según
-- Web.config) antes de dar por cerrado el rendimiento del ticket.

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.MSocios')
      AND name = 'IX_MSocios_dFechaActualizacion'
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_MSocios_dFechaActualizacion
        ON dbo.MSocios (dFechaActualizacion DESC);
END

-- Rendimiento de paginación en MMateriales (mismo patrón aplicado a MSocios).
--
-- dbo.MMateriales no tenía ningún índice sobre dFechaActualizacion, que es la columna de orden
-- por defecto del grid (ReadMMateriales ordena por ahí antes de paginar). Sin índice, una página
-- avanzada obliga a SQL Server a ordenar la tabla completa en cada petición.
--
-- Medido en local (CLUBS_AX_TEST, 49.634 filas en MMateriales):
--   Página avanzada (OFFSET 30000) sin índice: ~91 ms | con índice: ~8 ms
--
-- Aplicar en cada entorno donde exista CLUBS_AX_TEST antes de dar por cerrado el rendimiento.

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.MMateriales')
      AND name = 'IX_MMateriales_dFechaActualizacion'
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_MMateriales_dFechaActualizacion
        ON dbo.MMateriales (dFechaActualizacion DESC);
END

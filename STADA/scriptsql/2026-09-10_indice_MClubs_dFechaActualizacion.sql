-- Rendimiento de paginación en MClubs (mismo patrón aplicado a MSocios y MMateriales).
--
-- dbo.MClubs no tenía ningún índice sobre dFechaActualizacion, que es la columna de orden
-- por defecto del grid (ReadMClub/ReadMClubActivo ordenan por ahí). Sin índice, una página
-- avanzada obliga a SQL Server a ordenar la tabla completa en cada petición.
--
-- Aplicar en cada entorno donde exista CLUBS_AX_TEST antes de dar por cerrado el rendimiento.

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.MClubs')
      AND name = 'IX_MClubs_dFechaActualizacion'
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_MClubs_dFechaActualizacion
        ON dbo.MClubs (dFechaActualizacion DESC);
END

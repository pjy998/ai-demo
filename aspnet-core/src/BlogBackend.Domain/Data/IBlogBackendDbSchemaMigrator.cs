using System.Threading.Tasks;

namespace BlogBackend.Data;

public interface IBlogBackendDbSchemaMigrator
{
    Task MigrateAsync();
}

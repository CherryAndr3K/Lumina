using Lumina.Repositories;
using Lumina.Views;
using System;
using System.Windows;
using Lumina.Seeding;
namespace Lumina
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var repo = new RepositoryBase();
                using (var conn = repo.GetType()
                    .GetMethod("GetConnection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .Invoke(repo, null) as System.Data.SqlClient.SqlConnection)
                {
                    conn.Open();
                    Console.WriteLine($"Conexión exitosa a la BD: {conn.Database} en {conn.DataSource}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al conectar: " + ex.Message);
            }

            CrearLibros.Inicializar();

            var loginWindow = new Login();
            loginWindow.Show();
        }
    }
}


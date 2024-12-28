using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Logging;
using RegistroEmpleados.Modelos.Modelos;

namespace RegistroEmpleados.AppMovil
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            _ = ActualizarCursos();
            _ = ActualizarEstudiantes();
            return builder.Build();


        }

        public static async Task ActualizarCursos()
        {
            try
            {

                FirebaseClient client = new FirebaseClient("https://registroempleados-433b8-default-rtdb.firebaseio.com/");

                var cursos = await client.Child("Cursos").OnceAsync<Curso>();

                if (cursos.Count == 0)
                {
                    var listaDeCursos = new List<Curso>
                    {
                        new Curso { Nombre = "1ro Básico" },
                        new Curso { Nombre = "2do Básico" },
                        new Curso { Nombre = "3ero Básico" },
                        new Curso { Nombre = "4to Básico" },
                        new Curso { Nombre = "5to Básico" },
                        new Curso { Nombre = "6to Básico" },
                        new Curso { Nombre = "7mo Básico" },
                        new Curso { Nombre = "8vo Básico" },
                        new Curso { Nombre = "1er Medio" },
                        new Curso { Nombre = "2do Medio" },
                        new Curso { Nombre = "3ero Medio" },
                        new Curso { Nombre = "4to Medio" }
                    };

                    foreach (var curso in listaDeCursos)
                    {
                        await client.Child("Cursos").PostAsync(curso);
                    }
                }
                else
                {
                    foreach (var curso in cursos)
                    {
                        if (curso.Object.Estado == null)
                        {
                            var cursoActualizado = curso.Object;
                            cursoActualizado.Estado = true;

                            await client.Child("Cursos").Child(curso.Key).PutAsync(cursoActualizado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar cursos: {ex.Message}");
            }
        }
        public static async Task ActualizarEstudiantes()
        {
            try
            {
                FirebaseClient client = new FirebaseClient("https://registroempleados-433b8-default-rtdb.firebaseio.com/");
                var estudiantesExistentes = await client.Child("Estudiantes").OnceAsync<Estudiante>();

                foreach (var estudiante in estudiantesExistentes)
                {
                    if (estudiante.Object.Estado == null)
                    {
                        var estudianteActualizado = estudiante.Object;
                        estudianteActualizado.Estado = true;

                        await client.Child("Estudiantes").Child(estudiante.Key).PutAsync(estudianteActualizado);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar estudiantes: {ex.Message}");
            }
        }
    }

}

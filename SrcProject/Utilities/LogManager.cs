namespace SrcProject.Utilities
{
    public class LogManager
    {
        public static string DebugLog(string mensaje)
        {
            var wrote = "";
            try
            {
                DateTime dt = DateTime.Now;
                string s = dt.ToString("yyyyMMdd");

                // Obtener la ruta del directorio actual del proyecto
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string directoryPath = Path.Combine(baseDirectory, "App-Logs");
                string filePath = Path.Combine(directoryPath, "LogDebug_API_" + s + ".txt");

                // Verificar si la carpeta existe, si no, crearla
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (StreamWriter sw = new StreamWriter(filePath, true))
                {
                    sw.WriteLine("{0} >>>> {1}", DateTime.Now, mensaje);
                    sw.Flush();
                }
            }
            catch (Exception u)
            {
                wrote = u.Message.ToString();
            }
            return wrote;
        }
    }
}

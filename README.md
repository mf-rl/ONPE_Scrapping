# ONPE_Scrapping
Pequeña aplicación para obtener datos de las elecciones presidenciales de Perú, que es información pública y se encuentra publicada en la misma página de la ONPE.

Esta aplicación se encuentra en versión de pruebas. Si alguien quiere contribuir y mejorar, es bienvenido. 

## Requisitos previos
- .NET SDK 5.0 o superior ([Descargar aquí](https://dotnet.microsoft.com/download))
- Windows, Linux o macOS

## Instalación y configuración

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/your-username/ONPE_Scrapping.git
   cd ONPE_Scrapping
   ```

2. **Restaurar paquetes NuGet**
   ```bash
   dotnet restore
   ```

3. **Compilar el proyecto**
   ```bash
   dotnet build
   ```
   
   Nota: Este paso genera automáticamente el script `playwright.ps1` (o `playwright.sh` en Linux/macOS) en la carpeta `bin/Debug/net5.0`.

4. **Instalar navegadores Playwright** (requerido para bypass de Cloudflare)
   
   Después de compilar, el script de Playwright estará disponible en la carpeta de salida:
   
   En Windows (PowerShell):
   ```powershell
   cd PE_Scrapping\bin\Debug\net5.0
   .\playwright.ps1 install chromium
   ```
   
   En Linux/macOS:
   ```bash
   cd PE_Scrapping/bin/Debug/net5.0
   ./playwright.sh install chromium
   ```
   
   **Importante:** Este paso solo se requiere una vez. El navegador Chromium se descarga en `%USERPROFILE%\AppData\Local\ms-playwright` (Windows) o `~/.cache/ms-playwright` (Linux/macOS).

5. **Configurar appSettings.json** (opcional)
   
   Editar `PE_Scrapping/appSettings.json` para configurar:
   - Ruta de la base de datos SQLite
   - Ruta de guardado de archivos
   - Otros parámetros de configuración

6. **Ejecutar la aplicación**
   ```bash
   cd PE_Scrapping
   dotnet run
   ```

## Notas técnicas
- La aplicación utiliza **Playwright** para acceder a los datos de la ONPE, ya que el sitio web está protegido por Cloudflare
- Los datos se almacenan localmente en una base de datos SQLite
- Las actas se descargan como imágenes en la carpeta configurada

## Capturas de pantalla

![image](https://user-images.githubusercontent.com/73368752/122112210-3036c500-cde6-11eb-92be-c57fdc2bb254.png)

![image](https://user-images.githubusercontent.com/73368752/121822655-e1622180-cc65-11eb-9bb6-8846cd69602d.png)

![image](https://user-images.githubusercontent.com/73368752/121822731-3dc54100-cc66-11eb-8790-242a939f49db.png)

## APIs utilizadas<br />
1ra Vuelta<br />
Request URL: https://resultadoshistorico.onpe.gob.pe/v1/EG2021/ecp/ubigeos/T<br />
Request URL: https://resultadoshistorico.onpe.gob.pe/v1/EG2021/mesas/locales/010202<br />
Request URL: https://resultadoshistorico.onpe.gob.pe/v1/EG2021/mesas/actas/11/010202/0032<br />
Request URL: https://resultadoshistorico.onpe.gob.pe/v1/EG2021/mesas/detalle/000169<br />
<br />
2da Vuelta<br />
Request URL: https://resultadoshistorico.onpe.gob.pe/assets/json/SEP2021/ecp/ubigeos/T.json<br />
Request URL: https://resultadoshistorico.onpe.gob.pe/assets/json/SEP2021/mesas/locales/020206.json<br />
Request URL: https://resultadoshistorico.onpe.gob.pe/assets/json/SEP2021/mesas/actas/11/020206/I924.json<br />
Request URL: https://resultadoshistorico.onpe.gob.pe/assets/json/SEP2021/mesas/detalle/060486.json<br />

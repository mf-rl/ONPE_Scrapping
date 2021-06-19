# ONPE_Scrapping
Pequeña aplicación para obtener datos de las elecciones presidenciales de Perú, que es información pública y se encuentra publicada en la misma página de la ONPE.

Esta aplicación se encuentra en versión de pruebas. Si alguien quiere contribuir y mejorar, es bienvenido. 

Utiliza ChromeDriver versión 91. Verificar que chromedriver.exe se encuentre en la carpeta "runtimes", en la raiz de la aplicación. 
En el repositorio se encuentra en la carpeta "chromedriver_win32", pero también se puede configurar la ruta del chromedriver.exe en el archivo appSettings.json, bajo el parámetro "ChromeDriverPath".

Drive con data extraída. (Actualizando):<br />
https://drive.google.com/drive/folders/1OUv4eMdFjVXIxbN9wU8Bhn3FSvS521nK?usp=sharing

![image](https://user-images.githubusercontent.com/73368752/122112210-3036c500-cde6-11eb-92be-c57fdc2bb254.png)

![image](https://user-images.githubusercontent.com/73368752/121822655-e1622180-cc65-11eb-9bb6-8846cd69602d.png)

![image](https://user-images.githubusercontent.com/73368752/121822731-3dc54100-cc66-11eb-8790-242a939f49db.png)

Varias versiones de ChromeDriver, por si hay errores con la versión de Chrome:<br />
http://chromedriver.storage.googleapis.com/index.html

APIs utilizadas<br />
1ra Vuelta<br />
Request URL: https://resultadoshistorico.onpe.gob.pe/v1/EG2021/ecp/ubigeos/T<br />
Request URL: https://resultadoshistorico.onpe.gob.pe/v1/EG2021/mesas/locales/010202<br />
Request URL: https://resultadoshistorico.onpe.gob.pe/v1/EG2021/mesas/actas/11/010202/0032<br />
Request URL: https://resultadoshistorico.onpe.gob.pe/v1/EG2021/mesas/detalle/000169<br />
<br />
2da Vuelta<br />
Request URL: https://api.resultadossep.eleccionesgenerales2021.pe/ecp/ubigeos/T<br />
Request URL: https://api.resultadossep.eleccionesgenerales2021.pe/mesas/locales/020206<br />
Request URL: https://api.resultadossep.eleccionesgenerales2021.pe/mesas/actas/11/020206/I924<br />
Request URL: https://api.resultadossep.eleccionesgenerales2021.pe/mesas/detalle/060486<br />

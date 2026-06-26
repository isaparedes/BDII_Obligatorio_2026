# BDII Obligatorio 2026
#### Grupo 6: Vanesa Carballido, Josué Merino, Isabela Paredes

## Instrucciones:
- Tener instalado previamente .NET SDK 10.
   Instalación según el sistema operativo
   
    -  MAC:
   ```
   brew install --cask dotnet-sdk
   ```
    -  Windows:
   ```
   winget install Microsoft.DotNet.SDK.10
   ```
     - Linux:
   ```
   sudo apt-get update && sudo apt-get install -y dotnet-sdk-10.0
   ```

## Ejecución del backend:
- Ingresar en la carpeta fuente
   ```
   cd TicketingAPI
   ```
- Crear un archivo en la carpeta TicketingAPI llamado "appsettings.Development.json" utilizando de plantillla "appsettings.Example.json" modificando los datos correspondientes:

     - Server:
     ```
     mysql.reto-ucu.net
     ```
     - Port:
     ```
     50006
     ```
     - Database:
     ```
     CD_Grupo6
     ```
     - User:
     ```
     cd_g6_admin
     ```
     - Password:
     ```
     2026grupo6
     ```
     - Secret:
     ```
     Mundial2026VanesaJosueIsabelaBDII
     ```

- Guardar el archivo, luego correr el .NET SDK 10.
  
      dotnet run     
  
Ya está corriendo el backend.

## Ejecución del frontend:
Una vez corriendo el backend, accedemos al frontend en otra terminal nueva.

- Ingresar a la carpeta de frontend:
  
   ```
    cd frontend
   ```

- Luego abirmos el HTML:
     ```
    open index.html
    ```
- Opcional: se puede utilizar la extensión Live Server de Visual Studio Code

Automáticamente se abre el index.html donde se puede ver la pantalla de login.

## Prueba de funcionalidades:
Se pueden utilizar los diferentes usuarios creados en la base de datos para la prueba de las diferentes funcionalidades:

- Usuario General:
   - usuario: isa@example.com; contraseña: isa123
   - usuario: vane@example.com; contraseña: vane123
     
- Funcionario:
   - usuario: funcionario@fifa.com; contraseña: funcionario123
     
- Administrador:
   - usuario: adminUSA@example.com; contraseña: adminUSA123
   - usuario: adminMEX@example.com; contraseña: adminMEX123 (Este usuario es Usuario General y Administrador)
   - usuario: adminCAN@example.com; contraseña: adminCAN123




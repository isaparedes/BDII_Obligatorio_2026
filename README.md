# BDII Obligatorio 2026

#### Grupo 6: Vanesa Carballido, Josué Merino, Isabela Paredes

## Instrucciones

Tener instalado previamente **.NET SDK 10**.

### Instalación según el sistema operativo

* **macOS**

  ```bash
  brew install --cask dotnet-sdk
  ```

* **Windows**

  ```bash
  winget install Microsoft.DotNet.SDK.10
  ```

## Ejecución del backend

1. Ingresar a la carpeta del backend:

   ```bash
   cd TicketingAPI
   ```

2. Crear un archivo llamado **`appsettings.Development.json`** utilizando como plantilla **`appsettings.Example.json`**.

3. Completar los siguientes datos:

   ```text
   Server:   mysql.reto-ucu.net
   Port:     50006
   Database: CD_Grupo6
   User:     cd_g6_admin
   Password: 2026grupo6
   Secret:   Mundial2026VanesaJosueIsabelaBDII
   ```

4. Guardar el archivo y ejecutar:

   ```bash
   dotnet run
   ```

Una vez iniciado correctamente, el backend quedará en ejecución.

---

## Ejecución del frontend

Con el backend en ejecución, abrir una nueva terminal.

1. Ingresar a la carpeta del frontend:

   ```bash
   cd frontend
   ```

2. Abrir el frontend utilizando una de las siguientes opciones:

   **Opción recomendada:** utilizar la extensión **Live Server** de Visual Studio Code.

   **Alternativa (macOS):**

   ```bash
   open index.html
   ```

   **Alternativa (Windows):** abrir el archivo `index.html` directamente desde el navegador.

Al abrir la aplicación se mostrará la pantalla de inicio de sesión.

---

## Usuarios de prueba

### Usuario General

* **Usuario:** `isa@example.com`
  **Contraseña:** `isa123`

* **Usuario:** `vane@example.com`
  **Contraseña:** `vane123`

### Funcionario

* **Usuario:** `funcionario@fifa.com`
  **Contraseña:** `funcionario123`

### Administrador

* **Usuario:** `adminUSA@example.com`
  **Contraseña:** `adminUSA123`

* **Usuario:** `adminMEX@example.com`
  **Contraseña:** `adminMEX123` *(Administrador y Usuario General)*

* **Usuario:** `adminCAN@example.com`
  **Contraseña:** `adminCAN123`

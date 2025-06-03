# 🪓 Hachodromo

Aplicación multiplataforma para la gestión de reservas, sesiones y clientes en un local de lanzamiento de hachas. Desarrollado con .NET MAUI, Blazor y tecnologías modernas para backend y frontend.

Base de proyecto de FP DE DAM en Gijón AST- 
---

## 📁 Estructura del proyecto

Hachodromo/
│
├── Hachodromo.API # API REST principal
├── Hachodromo.Shared # Modelos y lógica compartida
│
├── Maui/ # Solución multiplataforma actual
│ ├── Maui/ # Aplicación MAUI (Android/iOS/Windows)
│ ├── Maui.Shared/ # Recursos compartidos entre capas
│ ├── Maui.Web/ # Proyecto Blazor Server o backend web
│ └── Maui.Web.Client/ # Cliente Blazor WebAssembly
│
├── z_archived/ # Soluciones obsoletas (MauiApp, MauiBlazorApp, etc.)
│
├── Hachodromo.sln # Solución principal
└── .gitignore



---

## 🚀 Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/)
- Visual Studio 2022 o superior con soporte para:
  - MAUI
  - ASP.NET Core
  - Blazor WebAssembly
- (Opcional) Emulador Android o dispositivo físico

---

## 🧪 Cómo ejecutar localmente

### 1. Restaurar dependencias
bash
dotnet restore

2. Ejecutar la API
bash
Copiar
Editar
cd Hachodromo.API
dotnet run

3. Ejecutar el cliente Blazor Web
bash
Copiar
Editar
cd Maui/Maui.Web.Client
dotnet run

4. Ejecutar la app MAUI (Windows, Android, etc.)
Abre Hachodromo.sln en Visual Studio
Selecciona el proyecto Maui
Elige la plataforma y pulsa Iniciar (F5)

📦 Soluciones archivadas
Las siguientes carpetas y proyectos han sido marcados como obsoletos y se encuentran en z_archived/. No forman parte activa del desarrollo actual:

MauiApp/

MauiBlazorApp/

🛡️ Licencia Proyecto Personal de José Gregorio Flores Silva para EL IESn1

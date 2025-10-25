# TalentBridge - Plataforma Inteligente de Empleabilidad Juvenil

## 📖 Descripción
TalentBridge es una plataforma web diseñada para facilitar la inserción laboral de jóvenes dominicanos mediante herramientas innovadoras de evaluación de CV, acceso a vacantes y generación de comunidad.

## 🚀 Características Implementadas

### Módulos del Backend
- **🔐 Autenticación**: Registro e inicio de sesión seguro
- **👥 Gestión de Usuarios**: CRUD completo de usuarios y perfiles
- **📄 Módulo CVs**: Subida, gestión y análisis de currículums con IA
- **💼 Bolsa de Trabajo**: Vacantes y sistema de aplicaciones
- **📊 Dashboard**: Métricas y seguimiento de postulaciones

### Tecnologías
- **Backend**: .NET 8, ASP.NET Core Web API
- **Base de Datos**: SQLite con Entity Framework Core
- **Arquitectura**: Clean Architecture + Domain-Driven Design
- **Autenticación**: Sistema personalizado con hashing SHA256
- **Documentación**: Swagger

## 🏗️ Estructura del Proyecto

```
TalentBridge/
├── TalentBridge.Domain/          # Entidades y modelos de dominio
├── TalentBridge.Application/     # Lógica de negocio y servicios  
├── TalentBridge.Infrastructure/  # Acceso a datos y repositorios
└── TalentBridge.WebAPI/          # Controladores y configuración
```

## 📡 Endpoints Principales

### Autenticación
- `POST /api/auth/register` - Registrar nuevo usuario
- `POST /api/auth/login` - Iniciar sesión

### Usuarios
- `GET /api/users` - Listar usuarios
- `POST /api/users` - Crear usuario
- `PUT /api/users/{id}` - Actualizar usuario
- `DELETE /api/users/{id}` - Eliminar usuario

### CVs
- `POST /api/cvs/upload` - Subir CV
- `GET /api/cvs/user/{userId}` - Obtener CVs del usuario
- `POST /api/cvs/analyze` - Analizar CV con IA

### Bolsa de Trabajo
- `GET /api/jobs` - Listar vacantes
- `POST /api/jobs` - Crear vacante
- `POST /api/jobs/apply` - Aplicar a vacante
- `GET /api/jobs/applications/user/{userId}` - Mis postulaciones

## 🛠️ Configuración y Ejecución

### Prerrequisitos
- .NET 8 SDK
- Visual Studio 2022 o VS Code

### Ejecución
1. Clonar el repositorio
2. Abrir la solución en Visual Studio
3. Establecer `TalentBridge.WebAPI` como proyecto de inicio
4. Ejecutar la aplicación (F5)
5. Acceder a Swagger: `https://localhost:7051/swagger`

### Configuración de Base de Datos
- Base de datos: SQLite (TalentBridge.db)
- Se crea automáticamente al ejecutar la aplicación
- Migraciones configuradas con Entity Framework Core

## 👥 Equipo de Desarrollo
- **Mayory Astacio Reyna** (2023-0272)
- **Scarlette Isabel Moya Hernández** (2023-0274) 
- **Nicolle Rosa Andújar** (2023-1075)
- **Bily Manuel Alvarez Sánchez** (2023-0952)

### Asesor
- **Willis Ezequiel Polanco Caraballo**

## 📄 Licencia
Proyecto académico para optar al título de Tecnología En Desarrollo De Software en el Instituto Tecnológico de las Américas (ITLA)

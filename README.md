# 📋 **TalentBridge - Plataforma Inteligente de Empleabilidad Juvenil**

![TalentBridge Logo](https://via.placeholder.com/800x300/4F46E5/FFFFFF?text=TalentBridge+Platform)

## 🎯 **Descripción del Proyecto**

**TalentBridge** es una plataforma web inteligente diseñada para facilitar la inserción laboral de jóvenes dominicanos mediante herramientas innovadoras de evaluación de CV con IA, acceso a vacantes laborales, generación de comunidad y mentorías profesionales.

### **Contexto del Problema**
- **Tasa de desempleo juvenil en RD**: 15.7% vs 5.5% general
- **Brezca educación-mercado laboral**: Falta de conexión entre formación académica y necesidades empresariales
- **Falta de orientación profesional**: Estudiantes necesitan guía para su desarrollo profesional
- **Poca visibilidad de talento joven**: Empresas no encuentran candidatos calificados

### **Objetivos del Proyecto**
1. Reducir la tasa de desempleo juvenil
2. Conectar estudiantes con oportunidades laborales
3. Proporcionar herramientas de desarrollo profesional
4. Crear comunidades de aprendizaje y networking
5. Ofrecer mentoría especializada por profesionales

---

## 🏗️ **Arquitectura del Sistema**

```
TalentBridge/
│
├── TalentBridge.API/                    # Backend API (.NET 8)
│   ├── Controllers/
│   │   ├── Auth/
│   │   │   ├── AuthController.cs
│   │   │   └── UsersController.cs
│   │   │
│   │   ├── Core/
│   │   │   ├── CVsController.cs
│   │   │   ├── JobsController.cs
│   │   │   └── ApplicationsController.cs
│   │   │
│   │   ├── University/
│   │   │   ├── UniversitiesController.cs
│   │   │   ├── CommunitiesController.cs
│   │   │   ├── CommunityPostsController.cs
│   │   │   └── CommunityCommentsController.cs
│   │   │
│   │   └── Mentorship/
│   │       ├── MentorshipsController.cs
│   │       ├── MentorshipSessionsController.cs
│   │       ├── MentorshipMilestonesController.cs
│   │       ├── MentorshipResourcesController.cs
│   │       ├── MentorshipRequestsController.cs
│   │       └── MentorsController.cs
│   │
│   ├── DTOs/
│   │   ├── AuthDTOs/
│   │   ├── CVDTOs/
│   │   ├── JobDTOs/
│   │   ├── UniversityDTOs/
│   │   ├── CommunityDTOs/
│   │   └── MentorshipDTOs/              # 20+ DTOs para mentorías
│   │
│   ├── Interfaces/
│   │   ├── IAuth/
│   │   ├── ICV/
│   │   ├── IJob/
│   │   ├── IUniversity/
│   │   ├── ICommunity/
│   │   └── IMentorship/                 # 8 interfaces de mentorías
│   │
│   ├── Services/
│   │   ├── AuthService.cs
│   │   ├── CVService.cs
│   │   ├── JobService.cs
│   │   ├── UniversityService.cs
│   │   ├── CommunityService.cs
│   │   ├── CommunityPostService.cs
│   │   ├── CommunityCommentService.cs
│   │   └── MentorshipServices/          # Servicio principal de mentorías
│   │       └── MentorshipService.cs
│   │
│   ├── Validations/
│   │   ├── AuthValidators/
│   │   ├── CVValidators/
│   │   ├── JobValidators/
│   │   ├── UniversityValidators/
│   │   ├── CommunityValidators/
│   │   └── MentorshipValidators/        # 13 validadores de mentorías
│   │       ├── CreateMentorshipValidator.cs
│   │       ├── UpdateMentorshipValidator.cs
│   │       ├── CreateSessionValidator.cs
│   │       ├── UpdateSessionValidator.cs
│   │       ├── CreateMilestoneValidator.cs
│   │       ├── UpdateMilestoneValidator.cs
│   │       ├── CreateResourceValidator.cs
│   │       ├── UpdateResourceValidator.cs
│   │       ├── CreateMentorshipRequestValidator.cs
│   │       ├── CreateMentorApplicationValidator.cs
│   │       ├── UpdateMentorProfileValidator.cs
│   │       ├── SearchMentorsValidator.cs
│   │       └── UpdateAttendanceValidator.cs
│   │
│   ├── Middleware/
│   ├── Helpers/
│   ├── Migrations/
│   ├── appsettings.json
│   ├── Program.cs
│   └── TalentBridge.db
│
├── TalentBridge.Domain/                 # Capa de Dominio
│   ├── Entities/
│   │   ├── Core/
│   │   │   ├── User.cs                  # Actualizado con propiedades de mentor
│   │   │   ├── CV.cs
│   │   │   ├── Job.cs
│   │   │   └── JobApplication.cs
│   │   │
│   │   ├── University/
│   │   │   ├── University.cs
│   │   │   ├── UniversityCareer.cs
│   │   │   ├── Community.cs
│   │   │   ├── CommunityMember.cs
│   │   │   ├── CommunityPost.cs
│   │   │   ├── CommunityComment.cs
│   │   │   ├── PostLike.cs
│   │   │   └── CommentLike.cs
│   │   │
│   │   └── Mentorship/                  # 7 entidades de mentorías
│   │       ├── Mentorship.cs
│   │       ├── MentorshipSession.cs
│   │       ├── SessionAttendance.cs
│   │       ├── MentorshipMilestone.cs
│   │       ├── MentorshipResource.cs
│   │       ├── MentorshipRequest.cs
│   │       └── MentorApplication.cs
│   │
│   └── Enums/
│       ├── UserRole.cs
│       ├── CommunityEnums.cs
│       └── MentorshipEnums.cs           # 8 enumerados de mentorías
│
├── TalentBridge.Infrastructure/         # Infraestructura
│   ├── Data/
│   │   ├── ApplicationDbContext.cs      # Configuración completa de todas las entidades
│   │   └── DatabaseSeeder.cs
│   │
│   ├── Migrations/
│   │   ├── 20250101_InitialCreate.cs
│   │   ├── 20250115_AddUniversityModule.cs
│   │   ├── 20250120_AddCommunityModule.cs
│   │   └── 20250125_AddMentorshipModule.cs
│   │
│   └── Repositories/
│       ├── Core/
│       │   ├── UserRepository.cs
│       │   ├── CVRepository.cs
│       │   ├── JobRepository.cs
│       │   └── ApplicationRepository.cs
│       │
│       ├── University/
│       │   ├── UniversityRepository.cs
│       │   ├── UniversityCareerRepository.cs
│       │   ├── CommunityRepository.cs
│       │   ├── CommunityMemberRepository.cs
│       │   ├── CommunityPostRepository.cs
│       │   ├── CommunityCommentRepository.cs
│       │   ├── PostLikeRepository.cs
│       │   └── CommentLikeRepository.cs
│       │
│       └── MentorshipRepositories/      # 7 repositorios de mentorías
│           ├── MentorshipRepository.cs
│           ├── MentorshipSessionRepository.cs
│           ├── SessionAttendanceRepository.cs
│           ├── MentorshipMilestoneRepository.cs
│           ├── MentorshipResourceRepository.cs
│           ├── MentorshipRequestRepository.cs
│           └── MentorApplicationRepository.cs
│
└── TalentBridge.WebAPI/                 # Frontend (React + Vite)
    ├── src/
    │   ├── components/
    │   │   ├── common/
    │   │   ├── auth/
    │   │   ├── cv/
    │   │   ├── jobs/
    │   │   ├── university/
    │   │   ├── community/
    │   │   └── mentorship/              # Componentes de mentorías
    │   │       ├── MentorProfile/
    │   │       ├── MentorshipRequests/
    │   │       ├── MentorshipSessions/
    │   │       └── MentorSearch/
    │   │
    │   ├── pages/
    │   │   ├── Auth/
    │   │   ├── Dashboard/
    │   │   ├── CV/
    │   │   ├── Jobs/
    │   │   ├── University/
    │   │   ├── Community/
    │   │   └── Mentorship/              # Páginas de mentorías
    │   │       ├── MentorDashboard/
    │   │       ├── MenteeDashboard/
    │   │       ├── SessionCalendar/
    │   │       └── MentorDirectory/
    │   │
    │   ├── services/
    │   │   ├── api/
    │   │   └── mentorshipService.js
    │   │
    │   └── App.jsx
    │
    ├── public/
    ├── package.json
    └── vite.config.js
```

---

## 🚀 **Características Principales**

### **1. 🎓 Módulo de Universidades**
- **Gestión de universidades**: CRUD completo de instituciones educativas
- **Carreras por universidad**: Asociación de programas académicos
- **Verificación institucional**: Sistema de validación de universidades
- **Estadísticas**: Número de estudiantes y comunidades por universidad

### **2. 👥 Módulo de Comunidades**
- **Comunidades universitarias**: Grupos por universidad (públicos/privados)
- **Sistema de miembros**: Roles (Owner, Admin, Moderator, Member)
- **Publicaciones y comentarios**: Foro interactivo con likes
- **Moderación**: Fijar posts, bloquear comentarios, remover miembros

### **3. 🎯 Módulo de Mentorías** *(NUEVO)*
- **Perfiles de mentores**: Expertos verificados con experiencia
- **Solicitudes de mentoría**: Estudiantes buscan orientación profesional
- **Sesiones programadas**: Calendario de reuniones (virtual/presencial)
- **Seguimiento de progreso**: Hitos y objetivos personalizados
- **Recursos compartidos**: Documentos, videos, enlaces educativos
- **Sistema de calificaciones**: Feedback bidireccional mentor-mentee

### **4. 📄 Evaluación de CV con IA**
- **Análisis automático**: Detección de fortalezas y áreas de mejora
- **Recomendaciones personalizadas**: Sugerencias basadas en industria
- **Checklist esencial**: Elementos clave para CV competitivo
- **Comparativa sectorial**: Benchmarking con candidatos similares

### **5. 💼 Bolsa de Trabajo Inteligente**
- **Matching automático**: Conexión CV-vacantes con algoritmos IA
- **Filtros avanzados**: Búsqueda por habilidades, experiencia, ubicación
- **Seguimiento de aplicaciones**: Estado de postulaciones en tiempo real
- **Alertas personalizadas**: Notificaciones de nuevas oportunidades

### **6. 📊 Dashboard Comunitario**
- **Métricas colectivas**: Estadísticas de empleabilidad de la comunidad
- **Logros compartidos**: Reconocimiento grupal por hitos alcanzados
- **Rankings universitarios**: Comparativa entre instituciones
- **Tendencias del mercado**: Insights de demanda laboral

---

## 🛠️ **Tecnologías Utilizadas**

### **Backend (.NET 8)**
- **ASP.NET Core 8**: Framework principal
- **Entity Framework Core 8**: ORM y migraciones
- **SQLite/PostgreSQL**: Bases de datos
- **JWT Bearer**: Autenticación y autorización
- **FluentValidation**: Validación de datos
- **AutoMapper**: Mapeo de objetos
- **Serilog**: Logging estructurado
- **Swagger/OpenAPI**: Documentación API

### **Frontend (React 18)**
- **React 18 + TypeScript**: Biblioteca principal
- **Vite**: Build tool y dev server
- **Tailwind CSS**: Framework de estilos
- **React Router v6**: Navegación
- **Axios**: Cliente HTTP
- **React Query**: Gestión de estado del servidor
- **Recharts/Chart.js**: Visualización de datos
- **React Hook Form**: Formularios
- **Zod**: Validación en frontend

### **Inteligencia Artificial**
- **OpenAI API**: Análisis de CVs y generación de contenido
- **Azure Cognitive Services**: Procesamiento de lenguaje natural
- **Custom ML Models**: Algoritmos de matching CV-vacantes

### **DevOps & Infrastructure**
- **Docker**: Contenedorización
- **GitHub Actions**: CI/CD Pipeline
- **Azure App Service/Heroku**: Hosting backend
- **Vercel/Netlify**: Hosting frontend
- **Azure SQL/PostgreSQL**: Base de datos en producción
- **Azure Blob Storage/S3**: Almacenamiento de archivos

---

## 📦 **Instalación y Configuración**

### **Requisitos Previos**
- .NET 8 SDK
- Node.js 18+ y npm/yarn
- Git
- SQL Server LocalDB o PostgreSQL
- Visual Studio 2022 o VS Code

### **1. Clonar Repositorio**
```bash
git clone https://github.com/tu-usuario/talentbridge.git
cd talentbridge
```

### **2. Configurar Backend**
```bash
cd TalentBridge.API

# Restaurar dependencias
dotnet restore

# Configurar base de datos
# Actualizar connection string en appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=TalentBridge.db" # SQLite
    # "DefaultConnection": "Server=localhost;Database=TalentBridge;Trusted_Connection=True;" # SQL Server
  }
}

# Aplicar migraciones
dotnet ef database update

# Ejecutar
dotnet run
```

### **3. Configurar Frontend**
```bash
cd TalentBridge.WebAPI

# Instalar dependencias
npm install

# Configurar variables de entorno
cp .env.example .env.local
# Editar .env.local con URLs del backend

# Ejecutar en desarrollo
npm run dev

# Build para producción
npm run build
```

### **4. Variables de Entorno Críticas**
```env
# Backend (.env o appsettings.json)
JWT__SecretKey=tu-clave-secreta-segura-minimo-32-caracteres
JWT__Issuer=TalentBridge
JWT__Audience=TalentBridgeUsers
OpenAI__ApiKey=sk-tu-api-key-de-openai
Azure__BlobStorageConnectionString=DefaultEndpointsProtocol=https;...

# Frontend (.env.local)
VITE_API_URL=http://localhost:5000/api
VITE_APP_NAME=TalentBridge
VITE_GOOGLE_ANALYTICS_ID=UA-XXXXX-Y
```

---

## 🔐 **Autenticación y Roles**

### **Roles del Sistema**
1. **Admin**: Acceso completo a toda la plataforma
2. **UniversityAdmin**: Gestión de una universidad específica
3. **Mentor**: Profesional que ofrece mentorías
4. **Mentee**: Estudiante que recibe mentorías
5. **Employer**: Representante de empresa (publica vacantes)
6. **Student**: Usuario regular (puede ser mentee)
7. **CommunityAdmin**: Administrador de comunidad

### **Endpoints de Autenticación**
- `POST /api/auth/register` - Registro de usuario
- `POST /api/auth/login` - Inicio de sesión
- `POST /api/auth/refresh-token` - Renovar token
- `POST /api/auth/logout` - Cerrar sesión
- `GET /api/auth/profile` - Perfil del usuario

---

## 📚 **Módulo de Universidades**

### **Endpoints Principales**
```
GET    /api/universities                 # Todas las universidades
GET    /api/universities/active          # Universidades activas
GET    /api/universities/{id}            # Universidad por ID
GET    /api/universities/{id}/details    # Detalles con carreras
POST   /api/universities                 # Crear (Admin only)
PUT    /api/universities/{id}            # Actualizar (Admin only)
DELETE /api/universities/{id}            # Eliminar (Admin only)
PATCH  /api/universities/{id}/verify     # Verificar universidad

GET    /api/universities/{id}/careers    # Carreras de universidad
POST   /api/universities/careers         # Crear carrera (Admin only)
```

---

## 👥 **Módulo de Comunidades**

### **Endpoints Principales**
```
# Comunidades
GET    /api/communities                  # Todas las comunidades
GET    /api/communities/public           # Comunidades públicas
GET    /api/communities/university/{id}  # Por universidad
GET    /api/communities/my-communities   # Mis comunidades (Auth)
GET    /api/communities/{id}             # Comunidad específica
GET    /api/communities/{id}/details     # Detalles completos
POST   /api/communities                  # Crear comunidad (Auth)
PUT    /api/communities/{id}             # Actualizar (Admin/Mod)
DELETE /api/communities/{id}             # Eliminar (Owner only)

# Miembros
GET    /api/communities/{id}/members     # Miembros de comunidad
POST   /api/communities/{id}/join        # Unirse (Auth)
POST   /api/communities/{id}/leave       # Salir (Auth)
PATCH  /api/communities/{id}/members/role # Cambiar rol (Admin)
DELETE /api/communities/{id}/members/{memberId} # Remover miembro

# Posts
GET    /api/communities/{id}/posts       # Posts de comunidad
GET    /api/posts/{postId}               # Post específico
POST   /api/communities/{id}/posts       # Crear post (Member)
PUT    /api/posts/{postId}               # Actualizar post (Author/Mod)
DELETE /api/posts/{postId}               # Eliminar post (Author/Mod)
POST   /api/posts/{postId}/toggle-pin    # Fijar/desfijar (Mod)
POST   /api/posts/{postId}/toggle-lock   # Bloquear/desbloquear (Mod)
POST   /api/posts/{postId}/like          # Dar like (Auth)
DELETE /api/posts/{postId}/like          # Quitar like (Auth)

# Comentarios
GET    /api/posts/{postId}/comments      # Comentarios de post
POST   /api/posts/{postId}/comments      # Crear comentario (Member)
PUT    /api/comments/{commentId}         # Actualizar (Author)
DELETE /api/comments/{commentId}         # Eliminar (Author/Mod)
POST   /api/comments/{commentId}/like    # Dar like (Auth)
DELETE /api/comments/{commentId}/like    # Quitar like (Auth)
```

---

## 🎓 **Módulo de Mentorías** *(COMPLETO)*

### **Características del Módulo**
✅ **Perfiles de mentores verificados**  
✅ **Sistema de solicitudes y aplicaciones**  
✅ **Calendario de sesiones programadas**  
✅ **Seguimiento de hitos y objetivos**  
✅ **Biblioteca de recursos educativos**  
✅ **Sistema de calificaciones bidireccional**  
✅ **Búsqueda y filtrado avanzado de mentores**  
✅ **Dashboard de progreso para mentees**  
✅ **Panel de gestión para mentores**  
✅ **Notificaciones y recordatorios automáticos**

### **Endpoints Principales**

#### **Mentorías**
```
GET    /api/mentorships/my-mentorships   # Mis mentorías (Auth)
GET    /api/mentorships/{id}             # Mentoría específica
GET    /api/mentorships/{id}/details     # Detalles completos
POST   /api/mentorships                  # Crear mentoría (Mentee)
PUT    /api/mentorships/{id}             # Actualizar (Mentor/Mentee)
POST   /api/mentorships/{id}/complete    # Completar (Mentor)
POST   /api/mentorships/{id}/cancel      # Cancelar (Mentor/Mentee)
```

#### **Sesiones**
```
GET    /api/mentorships/{id}/sessions    # Sesiones de mentoría
POST   /api/mentorships/{id}/sessions    # Crear sesión (Mentor)
PUT    /api/sessions/{id}                # Actualizar sesión (Mentor)
DELETE /api/sessions/{id}                # Eliminar sesión (Mentor)
POST   /api/sessions/{id}/start          # Iniciar sesión (Mentor)
POST   /api/sessions/{id}/end            # Finalizar sesión (Mentor)
GET    /api/sessions/{id}/attendances    # Asistencias de sesión
POST   /api/sessions/{id}/attendances/{userId} # Marcar asistencia
```

#### **Hitos (Milestones)**
```
GET    /api/mentorships/{id}/milestones  # Hitos de mentoría
POST   /api/mentorships/{id}/milestones  # Crear hito (Mentor)
PUT    /api/milestones/{id}              # Actualizar hito (Mentor)
DELETE /api/milestones/{id}              # Eliminar hito (Mentor)
POST   /api/milestones/{id}/complete     # Marcar como completado
```

#### **Recursos**
```
GET    /api/mentorships/{id}/resources   # Recursos de mentoría
POST   /api/mentorships/{id}/resources   # Crear recurso (Mentor/Mentee)
PUT    /api/resources/{id}               # Actualizar recurso (Creador)
DELETE /api/resources/{id}               # Eliminar recurso (Creador/Mentor)
```

#### **Solicitudes y Aplicaciones**
```
GET    /api/mentorship-requests          # Solicitudes públicas
GET    /api/mentorship-requests/my-requests # Mis solicitudes
POST   /api/mentorship-requests          # Crear solicitud (Mentee)
DELETE /api/mentorship-requests/{id}     # Eliminar solicitud (Mentee)
GET    /api/mentorship-requests/{id}/applications # Aplicaciones
POST   /api/mentorship-requests/{id}/apply # Aplicar como mentor
POST   /api/mentorship-requests/applications/{id}/accept # Aceptar aplicación
POST   /api/mentorship-requests/applications/{id}/withdraw # Retirar aplicación
```

#### **Perfiles de Mentores**
```
GET    /api/mentors/profile              # Mi perfil de mentor (Auth)
GET    /api/mentors/{userId}/profile     # Perfil de mentor específico
PUT    /api/mentors/profile              # Actualizar perfil (Auth)
GET    /api/mentors/search               # Buscar mentores (filtros)
GET    /api/mentors/recommended          # Mentores recomendados (Auth)
GET    /api/mentors                      # Todos los mentores
```

### **Flujo de Trabajo de Mentorías**
1. **Perfilación**: Usuario se registra como mentor o mentee
2. **Búsqueda**: Mentee busca mentores por categoría, experiencia, rating
3. **Solicitud**: Mentee crea solicitud de mentoría (específica o abierta)
4. **Aplicación**: Mentores aplican a solicitudes abiertas
5. **Selección**: Mentee elige mentor y se crea la relación
6. **Planificación**: Mentor establece sesiones y hitos
7. **Ejecución**: Realización de sesiones con seguimiento
8. **Evaluación**: Calificación y feedback al finalizar

---

## 📊 **Modelo de Datos Completo**

### **Entidades Principales**
1. **User**: Usuario del sistema (extendido con propiedades de mentor)
2. **University**: Institución educativa
3. **UniversityCareer**: Carrera universitaria
4. **Community**: Comunidad universitaria
5. **CommunityMember**: Miembro de comunidad
6. **CommunityPost**: Publicación en comunidad
7. **CommunityComment**: Comentario en post
8. **Mentorship**: Relación mentor-mentee
9. **MentorshipSession**: Sesión programada
10. **SessionAttendance**: Asistencia a sesión
11. **MentorshipMilestone**: Hito de mentoría
12. **MentorshipResource**: Recurso educativo
13. **MentorshipRequest**: Solicitud de mentoría
14. **MentorApplication**: Aplicación de mentor

### **Relaciones Clave**
- **User** → **MentorMentorships** (como mentor)
- **User** → **MenteeMentorships** (como mentee)
- **University** → **Communities** (comunidades asociadas)
- **Community** → **Members** (usuarios miembros)
- **Mentorship** → **Sessions** (sesiones programadas)
- **MentorshipRequest** → **Applications** (aplicaciones de mentores)

---

## 🚀 **Roadmap de Desarrollo**

### **✅ Fase 1: MVP (Completado)**
- [x] Sistema de autenticación y autorización
- [x] Evaluación de CV con IA
- [x] Bolsa de trabajo básica
- [x] Dashboard comunitario
- [x] Módulo de universidades
- [x] Sistema de comunidades

### **✅ Fase 2: Expansión (Completado)**
- [x] Módulo de mentorías completo
- [x] Sistema de sesiones programadas
- [x] Biblioteca de recursos educativos
- [x] Calendario integrado
- [x] Sistema de calificaciones
- [x] Búsqueda avanzada de mentores

### **🔄 Fase 3: Escalabilidad (En Progreso)**
- [ ] Integración con calendarios externos (Google, Outlook)
- [ ] Sistema de videollamadas integrado
- [ ] Gamificación y certificaciones
- [ ] Análisis predictivo de empleabilidad
- [ ] Marketplace de servicios profesionales
- [ ] API pública para integraciones

### **📅 Fase 4: Internacionalización**
- [ ] Soporte multi-idioma
- [ ] Expansión a otros países
- [ ] Integración con LinkedIn
- [ ] Aplicación móvil nativa
- [ ] Sistema de pagos para mentorías premium

---

## 👥 **Equipo de Desarrollo**

### **Desarrolladores**
- **Mayory Astacio Reyna** (2023-0272)
- **Scarlette Isabel Moya Hernández** (2023-0274) 
- **Nicolle Rosa Andújar** (2023-1075) 
- **Bily Manuel Alvarez Sánchez** (2023-0952)

### **Asesor**
- **Willis Ezequiel Polanco Caraballo** - Coordinador del Proyecto

### **Institución**
- **Instituto Tecnológico de las Américas (ITLA)**
- Centro de Excelencia de Software
- Santo Domingo Este, República Dominicana

---

## 📄 **Licencia**

Este proyecto es parte del trabajo de grado del **Instituto Tecnológico de las Américas (ITLA)** para optar al título de **Tecnología en Desarrollo de Software**.

**Propiedad Intelectual**: © 2024 TalentBridge Team. Todos los derechos reservados.

**Uso Académico**: Este proyecto puede ser utilizado con fines educativos y de investigación, citando adecuadamente a los autores.

**Restricciones**: No está permitida la comercialización o distribución sin autorización expresa del equipo desarrollador.

---

## 🤝 **Colaboración y Contribución**

### **Cómo Contribuir**
1. Fork el repositorio
2. Crear una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abrir un Pull Request

### **Código de Conducta**
- Respeto a todos los colaboradores
- Comentarios constructivos
- Documentación clara
- Testing antes de PR

### **Reportar Issues**
Usar el sistema de issues de GitHub con:
- Descripción detallada del problema
- Pasos para reproducir
- Comportamiento esperado vs actual
- Capturas de pantalla si aplica

---

## 📞 **Contacto y Soporte**

### **Información Institucional**
- **Institución**: Instituto Tecnológico de las Américas (ITLA)
- **Dirección**: Autopista Las Américas, Km. 27, PCSD, Santo Domingo Este
- **Teléfono**: (809) 738-4852
- **Website**: [www.itla.edu.do](https://www.itla.edu.do)

### **Equipo TalentBridge**
- **Email**: talentbridge.itla@gmail.com
- **GitHub**: [github.com/talentbridge-itla](https://github.com/talentbridge-itla)
- **LinkedIn**: [TalentBridge RD](https://linkedin.com/company/talentbridge-rd)

### **Horarios de Soporte**
- **Lunes a Viernes**: 8:00 AM - 5:00 PM (GMT-4)
- **Sábados**: 9:00 AM - 1:00 PM (GMT-4)
- **Respuesta en**: 24-48 horas hábiles

---

## 🙏 **Agradecimientos**

### **Instituciones Apoyadoras**
- **Instituto Tecnológico de las Américas (ITLA)**
- **Banco Central de la República Dominicana (BCRD)**
- **Banco Interamericano de Desarrollo (BID)**
- **Ministerio de Educación Superior, Ciencia y Tecnología (MESCYT)**

### **Mentores y Asesores**
- Todos los profesores del ITLA que brindaron su guía
- Profesionales de la industria que ofrecieron retroalimentación
- Compañeros de clase por su apoyo colaborativo

### **Tecnologías Open Source**
- .NET Foundation
- React Community
- OpenAI
- Comunidad de desarrolladores dominicanos

---

## ⚠️ **Notas Importantes**

### **Estado del Proyecto**
✅ **Backend**: 100% completado  
✅ **Base de Datos**: 100% modelado  
✅ **API Documentation**: 100% con Swagger  
🔄 **Frontend**: 70% completado  
🔄 **Testing**: 60% completado  
📅 **Despliegue**: En progreso  

### **Próximos Hitos**
1. **Enero 2025**: Completar frontend de mentorías
2. **Febrero 2025**: Testing integral y QA
3. **Marzo 2025**: Despliegue en producción
4. **Abril 2025**: Lanzamiento oficial

### **Recursos Adicionales**
- [Documentación API](https://api.talentbridge.docs)
- [Guía de Usuario](https://docs.talentbridge.guide)
- [Demo en Vivo](https://demo.talentbridge.app)
- [Presentación del Proyecto](https://slides.talentbridge.presentation)

---

**"Conectando el talento juvenil dominicano con las oportunidades del futuro"** 🚀

*Última actualización: Diciembre 2024*

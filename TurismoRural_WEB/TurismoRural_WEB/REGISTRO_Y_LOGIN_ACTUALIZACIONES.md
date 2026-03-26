# Registro y Login - Actualizaciones de la API

## Resumen de Cambios

Se ha actualizado el proyecto web (Razor Pages) para implementar **registro (Registro)** y **login (IniciarSesión)** usando la lógica actualizada de la API. Los datos del usuario se guardan ahora como **variables de sesión** para usar durante toda la sesión.

---

## Cambios Realizados

### 1. **UserApiService.cs** (Servicios)
- ✅ Actualizado `RegisterUserAsync()` para usar el endpoint correcto: `/Users/RegistroUsuario`
- ✅ Actualizado `LoginUserAsync()` para usar el endpoint correcto: `/Users/IniciarSesion`
- ✅ Cambio de modelo de respuesta de `UserViewModel` a `UsuarioResponse`
- ✅ Corrección de nombres de propiedades en el payload:
  - `nombre` → `nombre` (correo) → `correoElectronico`
  - `contrasena` → `contrasenna`
  - Adición de `telefono` opcional

### 2. **Models**

#### UserRegisterViewModel.cs
- ✅ Cambio de convención de nombres (camelCase → PascalCase):
  - `nombre` → `Nombre`
  - `correo` → `CorreoElectronico`
  - `contrasena` → `Contrasena`
  - `confirmarContrasena` → `ConfirmarContrasena`
  - `telefono` → `Telefono` (ahora opcional)
- ✅ Removido `id_Rol` (no es requerido en el registro, la API lo asigna por defecto)

#### UsuarioResponse.cs (Nuevo)
- ✅ Creado nuevo modelo para la respuesta de login que incluye:
  - `Id`: ID del usuario
  - `Nombre`: Nombre del usuario
  - `Correo`: Correo electrónico del usuario
  - `Token`: Token JWT para autenticación

### 3. **AccountController.cs** (Controlador)
- ✅ Actualización del método `Login()`:
  - Cambio de tipo de retorno de `UserViewModel` a `UsuarioResponse`
  - Ahora guarda las variables de sesión:
    - `UsuarioId` (Int32)
    - `NombreUsuario` (String)
    - `CorreoUsuario` (String)
    - `TokenUsuario` (String) - El token JWT
  - Mensaje de error mejorado

- ✅ Actualización del método `Register()`:
  - Mejora en el mensaje de éxito (ahora dice "Por favor inicie sesión")

### 4. **Vistas Razor**

#### Profile.cshtml
- ✅ Actualización para usar las nuevas variables de sesión:
  - Adición de `UsuarioId`
  - Uso de `TokenUsuario` (para futuras llamadas a API autenticadas)
  - Removido `RolUsuario` (no se incluye en la respuesta de login)
  - Agregado botón de "Cerrar Sesión"

#### Register.cshtml
- ✅ Actualización de nombres de propiedades en el formulario:
  - `nombre` → `Nombre`
  - `correo` → `CorreoElectronico`
  - `contrasena` → `Contrasena`
  - `confirmarContrasena` → `ConfirmarContrasena`
  - `telefono` → `Telefono`

---

## Variables de Sesión Disponibles

Después de un login exitoso, estarán disponibles las siguientes variables de sesión:

```csharp
// En cualquier controlador o vista
int usuarioId = HttpContext.Session.GetInt32("UsuarioId");
string nombre = HttpContext.Session.GetString("NombreUsuario");
string correo = HttpContext.Session.GetString("CorreoUsuario");
string token = HttpContext.Session.GetString("TokenUsuario");
```

### Ejemplo en Razor:
```html
@using Microsoft.AspNetCore.Http

@{
    var usuarioId = Context.Session.GetInt32("UsuarioId");
    var nombre = Context.Session.GetString("NombreUsuario");
    var correo = Context.Session.GetString("CorreoUsuario");
    var token = Context.Session.GetString("TokenUsuario");
}

<p>Bienvenido, @nombre (@correo)</p>
```

---

## Flujo de Autenticación

### Registro (Registro)
1. Usuario completa el formulario de registro
2. Se envía POST a `/api/Users/RegistroUsuario`
3. Si es exitoso, se redirige a Login con mensaje de éxito
4. La contraseña es encriptada por el servidor

### Login (IniciarSesión)
1. Usuario completa correo y contraseña
2. Se envía POST a `/api/Users/IniciarSesion`
3. API valida y retorna `UsuarioResponse` con Token JWT
4. Se guardan datos en variables de sesión
5. Se redirige a Perfil del usuario

### Logout
1. Se limpia la sesión (`HttpContext.Session.Clear()`)
2. Se redirige a Login

---

## Configuración Necesaria

### appsettings.json (ya configurado)
```json
{
  "Valores": {
    "UrlAPI": "https://localhost:7054/api/"
  }
}
```

### Program.cs (ya configurado)
```csharp
builder.Services.AddSession();
// ...
app.UseSession();
```

---

## Endpoints de la API

- **Registro**: `POST /api/Users/RegistroUsuario`
- **Login**: `POST /api/Users/IniciarSesion`
- **Cambiar Perfil**: `PUT /api/Seguridad/CambiarPerfil` (autenticado)
- **Cambiar Contraseña**: `PUT /api/Seguridad/CambiarAcceso` (autenticado)
- **Recuperar Acceso**: `PUT /api/Users/RecuperarAcceso`

---

## Próximos Pasos (Opcionales)

1. Implementar autenticación basada en JWT usando el token guardado
2. Crear un helper de sesión para acceder fácilmente a las variables
3. Implementar cambio de perfil y contraseña
4. Implementar recuperación de contraseña
5. Agregar persistencia de sesión (cookies)


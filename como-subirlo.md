# Subir el arranque de Lynx Gym a GitHub — paso a paso

Repo: https://github.com/jsanchez-null/lynx-gym (público, creado por Jose el 23 nov 2026,
vacío al verificarlo ese día). Esto no se subió todavía: Abi no tiene sesión de GitHub
en su máquina, así que los pasos los corre Jose en su PC.

## 1. Clonar el repo vacío

En una terminal, en la carpeta donde quieras el proyecto:

```bash
git clone https://github.com/jsanchez-null/lynx-gym.git
cd lynx-gym
```

## 2. Copiar el arranque

Copia dentro de `lynx-gym/` todo el contenido de la carpeta `repo-starter/`:
`README.md`, `.gitignore`, `app/`, `backend/` y `docs/`.

## 3. Subirlo

```bash
git add .
git commit -m "Arranque Lynx Gym: estructura app (React Native + Expo) y backend (ASP.NET Core 3 capas)"
git push origin main
```

Si `git push` pide usuario y contraseña, en GitHub se usa un Personal Access Token
como contraseña, no la contraseña de la cuenta. Si la rama se llama `master` en vez
de `main`, el último comando es `git push origin master`.

## 4. Verificar

Abre https://github.com/jsanchez-null/lynx-gym en el navegador: deben verse el
README, `app/` y `backend/`. Después Paul puede clonarlo igual con el paso 1.

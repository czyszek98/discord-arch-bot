# DiscordArchBot

Discord Arch Bot to bot Discord umożliwiający archiwizowanie filmów z YouTube (zarówno klasycznych filmów jak i Shorts). Po podaniu linku bot pobiera materiał i zapisuje go na zdalnym serwerze przez SFTP.

---

## Zmienne środowiskowe

| Zmienna        | Opis                                   |
| -------------- |----------------------------------------|
| DISCORD_TOKEN  | Token bota Discord                     |
| DISCORD_CHANNEL_NAME  | Nazwa kanału, na którym ma działać bot |
| SFTP_HOST      | Host serwera SFTP                      |
| SFTP_PORT      | Port SFTP                              |
| SFTP_USER      | Użytkownik SFTP                        |
| SFTP_PASSWORD  | Hasło SFTP                             |
| SFTP_ARCH_PATH | Folder docelowy na serwerze zdalnym    |

---

## Komendy bota

### `!ping`

Sprawdza czy bot działa poprawnie.

**Odpowiedź:**

```
pong
```

---

### `!arch <link>`

Archiwizuje film z YouTube.

Obsługiwane:

* standardowe filmy YouTube
* YouTube Shorts

Po wykonaniu:

1. Bot pobiera materiał
2. Przetwarza go do pliku wideo
3. Wysyła przez SFTP do katalogu `SFTP_ARCH_PATH`
4. Informuje o powodzeniu lub błędzie

---

## Uruchomienie w Dockerze

### Budowanie obrazu

W katalogu projektu:

```bash
docker build -t discord-arch-bot .
```

---

### Uruchomienie (docker run)

```bash
docker run -d \
  --name discord-arch-bot \
  -e DISCORD_TOKEN=TOKEN \
  -e DISCORD_CHANNEL_NAME=Archiwum \
  -e SFTP_HOST=host \
  -e SFTP_PORT=22 \
  -e SFTP_USER=user \
  -e SFTP_PASSWORD=password \
  -e SFTP_ARCH_PATH=/archive/youtube \
  restart=unless-stopped \
  
  czyszek98/discord-arch-bot:latest
```

---

### docker-compose

```yaml
version: "3.9"

services:
  discord-arch-bot:
    image: czyszek98/discord-arch-bot:latest
    container_name: discord-arch-bot
    restart: unless-stopped
    environment:
      DISCORD_TOKEN: "TOKEN"
      DISCORD_CHANNEL_NAME: "Archiwum"
      SFTP_HOST: "host"
      SFTP_PORT: "22"
      SFTP_USER: "user"
      SFTP_PASSWORD: "password"
      SFTP_ARCH_PATH: "/archive/youtube"
```

Uruchomienie:

```bash
docker compose up -d
```

---

## Uwagi

* Bot nie przechowuje plików lokalnie po wysłaniu
* W przypadku błędu SFTP plik nie zostanie zapisany
* Nazwy plików odpowiadają tytułowi filmu (sanityzowane)

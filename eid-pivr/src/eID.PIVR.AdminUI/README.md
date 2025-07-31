# EID.PIVR.AdminUI

## Ръководство

### Инсталиране на npm пакетите

Инсталирайте `npm` пакетите, описани в `package.json` като изпълните следната команда:

```bash
$ npm install
```

### Как да генерирате self-signed SSL сертификат за localhost:

#### Certificate authority (CA)

Създайте папка "cert" в основната папка на UI проекта и генерирайте `RootCA.pem`, `RootCA.key` & `RootCA.crt` с тези команди:
Ако нямате инсталиран openssl може да изпозлвате Git Bash терминала.

```bash
$ openssl req -x509 -nodes -new -sha256 -days 1024 -newkey rsa:2048 -keyout RootCA.key -out RootCA.pem -subj "//C=US\CN=Example-Root-CA"
```

```bash
$ openssl x509 -outform pem -in RootCA.pem -out RootCA.crt
```

#### Domain name certificate

Създайте файл `domains.ext`, който има следното съдържание:

```
authorityKeyIdentifier=keyid,issuer
basicConstraints=CA:FALSE
keyUsage = digitalSignature, nonRepudiation, keyEncipherment, dataEncipherment
subjectAltName = @alt_names
[alt_names]
DNS.1 = localhost
```

Генерирайте `localhost.key`, `localhost.csr`, and `localhost.crt` със следните команди:

```bash
$ openssl req -new -nodes -newkey rsa:2048 -keyout localhost.key -out localhost.csr -subj "//C=US\ST=YourState\L=YourCity\O=Example-Certificates\CN=localhost.local"
```

```bash
$ openssl x509 -req -sha256 -days 1024 -in localhost.csr -CA RootCA.pem -CAkey RootCA.key -CAcreateserial -extfile domains.ext -out localhost.crt
```

#### Trusting the Certificate authority (CA) on Windows

1. Отворете файла "RootCA.crt".
2. Щракнете върху бутона "Install Certificate".
3. Когато се появи "Certificate Import Wizard", изберете "Current User".
4. Изберете "Place all certificates in the following store", след това щракнете върху бутона "Browse" и изберете "Trusted Root Certification Authorities" и щракнете "Next".
5. Щракнете върху "Finish" и ще се появи диалогов прозорец за сигурност относно самоподписания сертификат, който трябва да инсталирате. Щракнете върху "Yes", за да инсталирате сертификата.

### Как да стартираме проекта локално

1. След като сте генерирали self-signed SSL сертификат за localhost стартирайте dev сървъра, като изпълните следната команда. Отворете `https://localhost:4200/` в браузъра си. При промяна на изходните файлове, приложението ще се презареди автоматично.

```bash
$ npm run start
```

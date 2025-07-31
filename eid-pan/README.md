# eID.PAN solution
ПАН - Подсистемата за автоматични нотификации

# Структура на solution-a
1. eID.PAN.API (.net6 app) - API Gateway. Получава и валидира HTTP заявки от други системи, подсистеми и служебен потребителски интерфейс. Подготвя съответните команди за Application-а и ги изпраща през RabbitMQ  
2. eID.PAN.API.Public (.net6 app) - Public API Gateway. Получава и валидира HTTP заявки от портала за граждани, мобилният портал за граждани и заявки за регистриране на системи и канали за комуникация от ДЕАУ. Подготвя съответните команди за Application-а и ги изпраща през RabbitMQ  
3. eID.PAN.Application (.net6 app) - Инстанцира service-ите от eID.PAN.Service. Предвиден да scale-ва при натоварвания.  
4. eID.PAN.Service (class library) - Бизнес логика и връзки с БД  
4.1. Unit Tests  
5. eID.PAN.Contracts (class library)  - Контракти, които се използват при пренос на данни (команди, event-и, резултати)

# Описание на основните компоненти
## API (1)
- Предназначен да изпълнява заявки от други системи и служебния потребителски интерфейс
- Получава и authenticate-ва с токен или сертификат HTTP Request-и  
- Прави Request/Reply към Application (3) през RabbitMQ. За CorrelationId се използва стойността на `Request-Id` HTTP header-а. Ако не е подаден - генерира нов Guid.  
- Връща стандартизирани success и error response-и. Статус кодовете варират според извършваната операция.  
- Получените данни биват връщани на клиента  
- Има настроен Swagger. Ползват се Swagger attribute-ите, за да се възползваме от по-добър UX. Следващ етап - генериране на API client-и (SDK).  

## Public API (2)
- Предназначен да изпълнява заявки от портала за граждани, мобилния портал за граждани и заявки за регистриране на системи и канали за комуникация от ДЕАУ
- Получава и authenticate-ва с токен или сертификат HTTP Request-и  
- Прави Request/Reply към Application (3) през RabbitMQ. За CorrelationId се използва стойността на `Request-Id` HTTP header-а. Ако не е подаден - генерира нов Guid.  
- Връща стандартизирани success и error response-и. Статус кодовете варират според извършваната операция.  
- Получените данни биват връщани на клиента  
- Има настроен Swagger. Ползват се Swagger attribute-ите, за да се възползваме от по-добър UX. Следващ етап - генериране на API client-и (SDK).  

## Application (3)
- Consumer-ва команди произхождащи от API (1) и Public API (2)
- Инстанцира Business Logic (4) service-и, изпълнява поисканата операция, предавайки CorrelationId-то, и отговаря  
- Винаги отговаря. Не хвърля exception-и. Не пълни error/dead-letter queue-та.

## Service (4)
- Осъществява достъп до нужните ресурси (бази данни, подсистеми и др.)  
- Обект на UnitTest-ове.

## Contracts (5)
- Рефериран от другите проекти  
- Използваните от solution-а интерфейси се намират в namespace `eID.{ModuleName}.Contracts`. Например: `eID.{ModuleName}.Contracts.Events`  
- Интерфейси, предназначени за външни модули са едно ниво навътре в `eID.{ModuleName}.Contracts.External.*`. Например: `eID.{ModuleName}.Contracts.External.Events`  
 Те ще имплементират `EntityName`, за контрол на името на exchange-а.  
- Трябва да имплементират `CorrelatedBy<Guid>`. `CorrelationId` се предава по цялата верига.  


# Комуникация с външни модули
По изискване: `компонентите могат да pub/sub` към различни event-и, за това конфигурираме втори bus, който ще publish-ва RawJson-и.   
Там където ще искаме да publish-ваме (най-често в `Service` или на друго място) ще inject-ваме `IPublicBus` и на него ще му викаме `Publish<T>` метода.  
Тези, които ще се интересуват от някои от event-ите ще трябва да си създадат temporary (препоръчително) queue под exchange-а eID.PAN.Contracts.Events:T

# Response codes
_Проблемите (4\*\*, 5\*\*) се описват съгласно [RFC7807](https://datatracker.ietf.org/doc/html/rfc7807)  

## Извличане на данни

`200` - при успешно изпълнена операция. Връща резултат.  
`400` - проблем с входни данни  
`500` - Unhandled exception  

## Изтриване на данни
_HTTP Verb:_ `DELETE`  
`204` - успешно изтриване
`400` - проблем с входни данни
`404` - липсващ запис
`500` - Unhandled exception

## Въвеждане на данни
_HTTP Verb:_ `POST`  
`201` - при успешно изпълнена операция. Връщане на `Id` на новия запис  
`400` - проблем с входни данни  
`409` - Данните вече съществуват  
`500` - Unhandled exception  

## Редактиране на съществуващ запис
_HTTP Verb:_ `PUT`  
`400` - проблем с входни данни  
`409` - данните вече съществуват  
`404` - липсващ запис   
`500` - Unhandled exception  

## Други кодове
Една част от кодовете ще се връщат от middleware-и на системата. Например: `401 Unauthorized`, `403 Forbidden`

## Database
`DbContext` се намира в проекта `eID.PAN.Service` За да се направи миграция е нужно да се намирате тук: `.\eID.PAN.Service>`
Командата за създаване на нова миграция е: `dotnet ef migrations add "Migration name"`
Командата за прилагане на миграцията е: `dotnet ef database update`

## Добавяне на тестови данни
1. Стартирай `eID.PAN.API.Public` и `eID.PAN.Application`
2. Отвори [Swagger](http://localhost:61003/swagger/index.html)
3. За добавяне на нова система: `POST`**/api/v1/Notifications/systems** `Request body` paste-ни
```json
{
	"systemName": "PUN",
    "translations": [
        {
            "language": "bg",
            "name": "Подсистема за управление на носителите"
        },
        {
            "language": "en",
            "name": "Subsystem to manage the cards"
        }
    ],
	"events": [
		{
			"code": "EId-is-ready-for-processing",
			"isMandatory": true,
			"translations": [
				{
					  "language": "bg",
					  "shortDescription": "Картата е готова за обработка",
					  "description": "Картата е готова за обработка и печат"
				},
				{
					  "language": "en",
					  "shortDescription": "The card is ready for processing",
					  "description": "The card is ready for processing and print"
				}
			]
		}
	]
}
```
4. Натискате `Execute`
5. За добавяне на нов канал за нотификации: `POST`**/api/v1/NotificationChannels** `Request body` paste-ни
```json
{
    "systemId": "C60C3471-A097-47FC-8067-A32983542EA1",
    "name": "SMS",
    "description" : "Some description",
    "callbackUrl": "callbackUrl",
    "price": 0,
    "infoUrl": "infoUrl",
    "translations": [
        {
	        "language": "bg",
	        "name": "Картата е готова за обработка",
	        "description": "Картата е готова за обработка и печат"
        },
        {
	        "language": "en",
	        "name": "The card is ready for processing",
	        "description": "The card is ready for processing and print"
        }
    ]
}
```
6. Approve на NotificationChannel `PUT`**/v1/NotificationChannels/{id-from-previous-operation}/approve**
7. Изпращане на нотификация `POST` **/v1/Notifications/send?testSystemName=PUN**
```json
{
  "eventCode": "EId-is-ready-for-processing",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "translations": [
    {
      "language": "bg",
      "message": "Картата е готова за обработка"
    },
    {
      "language": "en",
      "message": "The card is ready for processing and print"
    }
  ]
}
```
# UI стартиране
1. В конзола стигате до папката на UI `..\src\eID.PAN.AdminUI`
2. Изпълнявате:
  - `npm install`
  - `npm start`
3. Отваряте линка [http://localhost:4200/](http://localhost:4200/)

# URLs
[API URL](http://localhost:60003/api/v1) _http://localhost:60003/api/v1_  
[API failing request](http://localhost:60003/api/v1/People?firstName=bad&lastName=bad) _http://localhost:60003/api/v1/People?firstName=bad&lastName=bad_  
[Swagger URL](http://localhost:60003/swagger) _http://localhost:60003/swagger_  
[API Health check](http://localhost:60003/health/ready) _http://localhost:60003/health/ready_  
[Public API URL](http://localhost:61003/api/v1) _http://localhost:61003/api/v1_  
[API Swagger URL](http://localhost:61003/swagger) _http://localhost:61003/swagger_  
[API Health check](http://localhost:61003/health/ready) _http://localhost:61003/health/ready_  
[Application metrics URL](http://localhost:60004/metrics) _http://localhost:60004/metrics_   
[Application Health check](http://localhost:60004/health/ready) _http://localhost:60004/health/ready_

# Концепции използвани в проекта
## Прехвърляне на данните от Service към Application
Резултатите върнати от изпълнението на операции в `Service` се връщат като `Interface`. 
Това е свързано с комуникацията между `API` и `Application`. 
Връзката между тях става посредством `Interfaces` които се намират в `Contracts`.
За да прехврълим данни от `Service` към `Application` -> `API` начина е 
`Entity` което описва базата данни да наследи `Contract` който служи за пренасяне на данни до `Application` -> `API`
По тоя начин нямаме задължение данните които се изискват от `API` да са 1:1 с `Entity`.

# Настройване на локална среда
## Настройка на Git hook-ове

За да активираш автоматичното управление на версиите, изпълни следната команда от **основната директория на проекта**:

```bash
./setup-hooks.sh

## Visual Studio 2022
Visual Studio 2022 from [https://my.visualstudio.com](https://my.visualstudio.com).  

## .NET Core SDK
1. Изтегли .NET 6 SDK от [тук](https://dotnet.microsoft.com/download).  
2. Инсталирай го.

## Entity Framework 7
В Command Prompt изпълни `dotnet tool install --global dotnet-ef --version 7.*`

## PostgreSQL for Windows
1. Изтеглете от [тук](https://www.enterprisedb.com/downloads/postgres-postgresql-downloads) версия 15.2 Version Windows x86-64
2. Стартирайте postgresql-15.2-\*-windows-x64.exe
3. Изберете:
- user: postgres (standard)
- pass: eid$pass 
- port: 5432     (standard)
- [Default locate]
За връзка с PostgreSQL използвайте **pgAdmin**

## Docker for Windows
1. Изтегли Docker for Windows от [тук](https://hub.docker.com/editions/community/docker-ce-desktop-windows).
2. Инсталирай го.

## RabbitMQ
В Command Prompt изпълни `docker run --name rabbitmq -p 5672:5672 -p 15672:15672 -d --restart unless-stopped masstransit/rabbitmq`  
_Внимание:_ Използва се [`masstransit/rabbitmq`](https://hub.docker.com/r/masstransit/rabbitmq) image.  
Той надгражда официалния RabbitMQ image с `delayed exchange plugin`, който е нужен за безпроблемната работа на [schedule](https://masstransit.io/documentation/configuration/scheduling) функционалността на `MassTransit`. 


## Redis
В Command Prompt изпълни `docker run --name redis -p 6379:6379 -d --restart unless-stopped redis`  

## Redis desktop tool
_По избор_ [Another Redis Desktop Manager](https://github.com/qishibo/AnotherRedisDesktopManager)

## Резервиране на портове в Windows
1. Отвори папката на repository-то.
2. Отвори `\ServicePorts ExcluceAutoBindOurSrvPorts.txt`. Копирай съдържанието му.
3. Отвори Command Prompt като администратор.
4. Paste-ни копираното и го изпълни (натисни `Enter`)

# Описание на APIs
1. eID.PAN.API - за админинстраторите от МВР, Service to Service communication
1. eID.PAN.API.Public - за граждани
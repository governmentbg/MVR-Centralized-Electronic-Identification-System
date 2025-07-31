# PanSdk - PHP комплект със софтуерни компоненти за интеграция за eID - PAN Public HTTP API

Подсистема за автоматични нотификации (ПАН) Публична


## Инсталация и употреба

### Изисквания

PHP 8.1 или по-нова версия.

### Composer

За да инсталирате обвързванията чрез [Composer](https://getcomposer.org/), добавете следното в `composer.json`:

```json
{
  "repositories": [
    {
      "type": "vcs",
      "url": "https://github.com/GIT_USER_ID/GIT_REPO_ID.git"
    }
  ],
  "require": {
    "GIT_USER_ID/GIT_REPO_ID": "*@dev"
  }
}
```

След това изпълнете `composer install`

### Ръчна инсталация

Изтеглете файловете и включете `autoload.php`:

```php
<?php
require_once('/path/to/OpenAPIClient-php/vendor/autoload.php');
```

## Първи стъпки

Моля, следвайте [процедурата по инсталация](#installation--usage) и след това изпълнете следното:

```php
<?php
require_once(__DIR__ . '/vendor/autoload.php');



// Конфигуриране на Bearer (JWT) оторизация
$config = PanSdk\Configuration::getDefaultConfiguration()->setAccessToken('YOUR_ACCESS_TOKEN');


$apiInstance = new PanSdk\Api\NotificationChannelsApi(    
    // Ако искате да използвате собствен http клиент, подайте ваш клиент, който имплементира `GuzzleHttp\ClientInterface`.
    // Това е по избор — по подразбиране ще се използва `GuzzleHttp\Client`.
    new GuzzleHttp\Client(),
    $config
);
$page_size = 100; // int | 
$page_index = 1; // int | 
$channel_name = 'channel_name_example'; // string | 

try {
    $result = $apiInstance->apiV1NotificationChannelsGet($page_size, $page_index, $channel_name);
    print_r($result);
} catch (Exception $e) {
    echo 'Exception when calling NotificationChannelsApi->apiV1NotificationChannelsGet: ', $e->getMessage(), PHP_EOL;
}

```

## Документация за API крайните точки

Всички URI адреси са относителни спрямо *https://panapipublic.sandbox.bgeid.bg*

Class | Method | HTTP request | Description
------------ | ------------- | ------------- | -------------
*NotificationChannelsApi* | [**apiV1NotificationChannelsGet**](docs/Api/NotificationChannelsApi.md#apiv1notificationchannelsget) | **GET** /api/v1/NotificationChannels | Get all notification channels
*NotificationChannelsApi* | [**apiV1NotificationChannelsIdPut**](docs/Api/NotificationChannelsApi.md#apiv1notificationchannelsidput) | **PUT** /api/v1/NotificationChannels/{id} | Update existing approved notification channel.   New version is added in Pending table. Name must be unique against approved channels.
*NotificationChannelsApi* | [**apiV1NotificationChannelsSelectedGet**](docs/Api/NotificationChannelsApi.md#apiv1notificationchannelsselectedget) | **GET** /api/v1/NotificationChannels/selected | Get user selected notification channels
*NotificationChannelsApi* | [**apiV1NotificationChannelsSelectionPost**](docs/Api/NotificationChannelsApi.md#apiv1notificationchannelsselectionpost) | **POST** /api/v1/NotificationChannels/selection | Register selection of user notification channels
*NotificationChannelsApi* | [**registerAsync**](docs/Api/NotificationChannelsApi.md#registerasync) | **POST** /api/v1/NotificationChannels | Add new notification channel in Pending table. Name must be unique against approved channels.
*NotificationsApi* | [**apiV1NotificationsDeactivatePost**](docs/Api/NotificationsApi.md#apiv1notificationsdeactivatepost) | **POST** /api/v1/Notifications/deactivate | Register deactivated user events
*NotificationsApi* | [**apiV1NotificationsDeactivatedGet**](docs/Api/NotificationsApi.md#apiv1notificationsdeactivatedget) | **GET** /api/v1/Notifications/deactivated | Get deactivated user notifications
*NotificationsApi* | [**apiV1NotificationsGet**](docs/Api/NotificationsApi.md#apiv1notificationsget) | **GET** /api/v1/Notifications | Get all Systems and notifications
*NotificationsApi* | [**registerOrUpdateSystemAsync**](docs/Api/NotificationsApi.md#registerorupdatesystemasync) | **POST** /api/v1/Notifications/systems | Register or update a system with its events.
*NotificationsApi* | [**sendNotificationAsync**](docs/Api/NotificationsApi.md#sendnotificationasync) | **POST** /api/v1/Notifications/send | Send notification to a user via users&#39; selected channels or fallback to default(SMTP) channel.

## Модели

- [GuidIPaginatedData](docs/Model/GuidIPaginatedData.md)
- [IdentifierType](docs/Model/IdentifierType.md)
- [NotificationChannelPayload](docs/Model/NotificationChannelPayload.md)
- [NotificationChannelTranslationRequest](docs/Model/NotificationChannelTranslationRequest.md)
- [NotificationChannelTranslationResult](docs/Model/NotificationChannelTranslationResult.md)
- [ProblemDetails](docs/Model/ProblemDetails.md)
- [RegisterNotificationChannelRequest](docs/Model/RegisterNotificationChannelRequest.md)
- [RegisterSystemRequest](docs/Model/RegisterSystemRequest.md)
- [RegisteredSystemResult](docs/Model/RegisteredSystemResult.md)
- [RegisteredSystemResultIPaginatedData](docs/Model/RegisteredSystemResultIPaginatedData.md)
- [RegisteredSystemTranslationRequest](docs/Model/RegisteredSystemTranslationRequest.md)
- [RegisteredSystemTranslationResult](docs/Model/RegisteredSystemTranslationResult.md)
- [SendNotificationRequestInput](docs/Model/SendNotificationRequestInput.md)
- [SystemEventRequest](docs/Model/SystemEventRequest.md)
- [SystemEventResult](docs/Model/SystemEventResult.md)
- [TranslationRequest](docs/Model/TranslationRequest.md)
- [TranslationResult](docs/Model/TranslationResult.md)
- [UserNotificationChannelResult](docs/Model/UserNotificationChannelResult.md)
- [UserNotificationChannelResultIPaginatedData](docs/Model/UserNotificationChannelResultIPaginatedData.md)
- [ValidationProblemDetails](docs/Model/ValidationProblemDetails.md)

## Оторизация

Схеми за автентикация, дефинирани за API-то:
### Bearer

- **Тип**: Bearer authentication (JWT)

## Тестове

За да стартирате тестовете, използвайте:

```bash
composer install
vendor/bin/phpunit
```

## За този пакет

- API version: `v1`
    - Generator version: `7.13.0`
- Build package: `org.openapitools.codegen.languages.PhpClientCodegen`

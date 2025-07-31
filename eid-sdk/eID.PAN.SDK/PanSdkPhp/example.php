<?php

require_once __DIR__ . '/vendor/autoload.php';

use GuzzleHttp\Client;
use GuzzleHttp\Exception\RequestException;
use PanSdk\Api\NotificationChannelsApi;
use PanSdk\Configuration;

// === AUTHENTICATION ===
$keycloakUrl = 'YOUR_KEYCLOAK_GET_TOKEN_URL';

$authParams = [
    'grant_type' => '',
    'username' => '',
    'password' => '',
    'client_id' => '',
    'client_secret' => ''
];

$httpClient = new Client();

try {
    echo "Authenticating...\n";
    $response = $httpClient->post($keycloakUrl, [
        'form_params' => $authParams,
        'headers' => ['Accept' => 'application/json']
    ]);
    $body = json_decode($response->getBody(), true);
    $accessToken = $body['access_token'] ?? '';
} catch (RequestException $e) {
    echo "Authentication failed: " . $e->getMessage() . "\n";
    exit(1);
}

// === API CALL ===
$apiUrl = 'API_URL';
$config = Configuration::getDefaultConfiguration()->setHost($apiUrl)->setAccessToken($accessToken);
$apiInstance = new NotificationChannelsApi(null, $config);

try {
    echo "Calling endpoint...\n";
    $result = $apiInstance->apiV1NotificationChannelsGet(100, 1);
    echo json_encode($result, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE);
} catch (Exception $e) {
    echo 'API Exception: ', $e->getMessage(), PHP_EOL;
}

echo "\nPress ENTER to exit...";
fgets(STDIN);

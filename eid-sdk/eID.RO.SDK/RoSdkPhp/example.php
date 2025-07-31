<?php

require_once __DIR__ . '/vendor/autoload.php';

use GuzzleHttp\Client;
use RoSdk\Api\DeauApi;
use RoSdk\Configuration;
use RoSdk\Model\ApproveEmpowermentByDeauRequest;

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
    if (!$accessToken) {
        throw new Exception("No access token received.");
    }
} catch (Exception $e) {
    echo "Authentication failed: " . $e->getMessage() . "\n";
    exit(1);
}

// === API CALL ===
$apiUrl = 'API_URL';
$config = Configuration::getDefaultConfiguration()
    ->setHost($apiUrl)
    ->setAccessToken($accessToken);

$apiInstance = new DeauApi(null, $config);

try {
    echo "Calling endpoint...\n";
    $request = new ApproveEmpowermentByDeauRequest([
        'empowermentId' => '00000000-0000-0000-0000-000000000000' // GUID.Empty
    ]);
    $result = $apiInstance->approveEmpowermentByDeauAsync($request);
    echo json_encode($result, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE);
} catch (Exception $e) {
    echo 'API Exception: ', $e->getMessage(), PHP_EOL;
}

echo "\nPress ENTER to exit...";
fgets(STDIN);

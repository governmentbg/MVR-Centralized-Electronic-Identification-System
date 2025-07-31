import eid.sdk.ro.ApiClient;
import eid.sdk.ro.ApiException;
import eid.sdk.ro.Configuration;
import eid.sdk.ro.api.DeauApi;
import eid.sdk.ro.model.ApproveEmpowermentByDeauRequest;
import eid.sdk.ro.model.ApproveEmpowermentByDeauResponse;

import okhttp3.*;
import com.google.gson.*;

import java.io.IOException;
import java.util.Scanner;
import java.util.UUID;

public class Example {
    public static void main(String[] args) {
        // === AUTHENTICATION ===
        String keycloakUrl = "YOUR_KEYCLOAK_GET_TOKEN_URL";
        OkHttpClient authClient = new OkHttpClient();

        RequestBody formBody = new FormBody.Builder()
                .add("grant_type", "")
                .add("username", "")
                .add("password", "")
                .add("client_id", "")
                .add("client_secret", "")
                .build();

        Request authRequest = new Request.Builder()
                .url(keycloakUrl)
                .post(formBody)
                .addHeader("Accept", "application/json")
                .build();

        String token = "";
        try (Response response = authClient.newCall(authRequest).execute()) {
            if (!response.isSuccessful()) {
                System.out.println("Authentication failed: " + response);
                return;
            }
            String responseBody = response.body().string();
            JsonObject json = JsonParser.parseString(responseBody).getAsJsonObject();
            token = json.get("access_token").getAsString();
        } catch (IOException e) {
            System.out.println("Exception during auth: " + e.getMessage());
            return;
        }

        // === API CALL ===
        String apiUrl = "API_URL";
        ApiClient defaultClient = Configuration.getDefaultApiClient();
        defaultClient.setBasePath(apiUrl);
        defaultClient.setAccessToken(token);

        DeauApi apiInstance = new DeauApi(defaultClient);

        try {
            System.out.println("Calling endpoint...");
            ApproveEmpowermentByDeauRequest request = new ApproveEmpowermentByDeauRequest();
            request.setEmpowermentId(UUID.fromString("00000000-0000-0000-0000-000000000000")); // GUID.Empty

            ApproveEmpowermentByDeauResponse response = apiInstance.approveEmpowermentByDeauAsync(request);

            Gson gson = new GsonBuilder().setPrettyPrinting().create();
            System.out.println(gson.toJson(response));
        } catch (ApiException e) {
            System.err.println("API Exception: " + e.getResponseBody());
        }

        System.out.println("\nPress ENTER to exit...");
        Scanner scanner = new Scanner(System.in);
        scanner.nextLine();
    }
}

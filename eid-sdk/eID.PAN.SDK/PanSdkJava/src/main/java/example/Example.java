import eid.sdk.pan.Api.NotificationChannelsApi;
import eid.sdk.pan.ApiClient;
import eid.sdk.pan.ApiException;
import eid.sdk.pan.Configuration;

import okhttp3.*;
import com.google.gson.*;

import java.io.IOException;
import java.util.Scanner;

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

        NotificationChannelsApi apiInstance = new NotificationChannelsApi(defaultClient);

        try {
            System.out.println("Calling endpoint...");
            var response = apiInstance.apiV1NotificationChannelsGet(100, 1);
            Gson gson = new GsonBuilder().setPrettyPrinting().create();
            System.out.println(gson.toJson(response));
        } catch (ApiException e) {
            System.err.println("API Exception: " + e.getMessage());
        }

        System.out.println("\nPress ENTER to exit...");
        Scanner scanner = new Scanner(System.in);
        scanner.nextLine();
    }
}

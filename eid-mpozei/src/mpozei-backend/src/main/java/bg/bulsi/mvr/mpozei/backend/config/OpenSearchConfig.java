package bg.bulsi.mvr.mpozei.backend.config;

import org.opensearch.client.RestHighLevelClient;
import org.opensearch.data.client.orhlc.AbstractOpenSearchConfiguration;
import org.opensearch.data.client.orhlc.ClientConfiguration;
import org.opensearch.data.client.orhlc.OpenSearchRestTemplate;
import org.opensearch.data.client.orhlc.RestClients;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.ssl.SslBundle;
import org.springframework.boot.ssl.SslBundles;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.data.elasticsearch.core.ElasticsearchOperations;

import java.time.Duration;
import java.util.List;

@Configuration
public class OpenSearchConfig extends AbstractOpenSearchConfiguration {
    @Value("${opensearch.hosts}")
    private String[] opensearchHosts;

    @Value("${opensearch.username}")
    private String opensearchUsername;

    @Value("${opensearch.password}")
    private String opensearchPassword;

//    @Autowired
//    private SslBundles sslBundles;

    @Bean
    @Override
    public RestHighLevelClient opensearchClient() {
     //   SslBundle sslBundle = sslBundles.getBundle("open-search-m2m");
        final ClientConfiguration clientConfiguration = ClientConfiguration.builder()
                .connectedTo(opensearchHosts)
                .usingSsl()
 //               .usingSsl(sslBundle.createSslContext())
                .withBasicAuth(opensearchUsername, opensearchPassword)
                .withConnectTimeout(Duration.ofSeconds(5))
                .withSocketTimeout(Duration.ofSeconds(3))
                .build();

        return RestClients.create(clientConfiguration).rest();
    }

    @Bean
    public ElasticsearchOperations elasticsearchOperations(RestHighLevelClient client) {
        return new OpenSearchRestTemplate(client);
    }
}

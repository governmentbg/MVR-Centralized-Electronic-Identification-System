package bg.bulsi.mvr.mpozei.model.opensearch;

import lombok.Getter;
import lombok.Setter;
import org.springframework.data.annotation.Id;
import org.springframework.data.elasticsearch.annotations.Document;

import java.util.List;
import java.util.UUID;

@Getter
@Setter
@Document(indexName = "html_attachment_index")
public class HtmlFileAttachment {
    @Id
    private String id = UUID.randomUUID().toString();
    private String fileName;
    private List<String> metaTags;
}

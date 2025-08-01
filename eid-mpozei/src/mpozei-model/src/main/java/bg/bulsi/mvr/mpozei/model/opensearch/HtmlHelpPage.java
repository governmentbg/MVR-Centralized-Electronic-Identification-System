package bg.bulsi.mvr.mpozei.model.opensearch;

import lombok.Getter;
import lombok.Setter;
import org.springframework.data.annotation.Id;
import org.springframework.data.elasticsearch.annotations.Document;

import java.util.List;
import java.util.UUID;

@Getter
@Setter
@Document(indexName = "html_page_index")
public class HtmlHelpPage {
    @Id
    private String id = UUID.randomUUID().toString();
    private String title;
    private String content;
    private String contentWithHtml;
    private String language;
    private String pageName;
    private List<HtmlFileAttachment> attachments;
}
package bg.bulsi.mvr.mpozei.model.repository;

import bg.bulsi.mvr.mpozei.model.opensearch.HtmlHelpPage;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.elasticsearch.annotations.Query;
import org.springframework.data.elasticsearch.repository.ElasticsearchRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface HtmlHelpPageRepository extends ElasticsearchRepository<HtmlHelpPage, String> {

    @Query("""
            {
              "bool": {
                "must": [
                  { "query_string": { "query": "?0"  } },
                  { "term": { "language": "?1"  } }
                ]
              }
            }
            """)
    Page<HtmlHelpPage> findAllByTitleOrContentOrMetaTags(String filter, String language, Pageable pageable);

    boolean existsByPageNameAndLanguage(String pageName, String language);

    List<HtmlHelpPage> findByPageName(String pageName);
}

package bg.bulsi.mvr.mpozei.backend.service;

import bg.bulsi.mvr.mpozei.contract.dto.FindHelpPagesFilter;
import bg.bulsi.mvr.mpozei.contract.dto.HelpPageDTO;
import bg.bulsi.mvr.mpozei.model.opensearch.HtmlHelpPage;
import org.springframework.data.domain.Page;

import java.util.List;

public interface OpenSearchService {
    HtmlHelpPage createHelpPage(HtmlHelpPage page);

    HtmlHelpPage getHelpPageById(String id);

    List<HtmlHelpPage> getHelpPagesByPageName(String pageName);

    HtmlHelpPage updateHelpPage(HelpPageDTO page);

    Page<HtmlHelpPage> findHelpPagesByFilter(FindHelpPagesFilter filter);

    void deleteHelpPageById(String id);

    List<HtmlHelpPage> getAllHelpPages();
}

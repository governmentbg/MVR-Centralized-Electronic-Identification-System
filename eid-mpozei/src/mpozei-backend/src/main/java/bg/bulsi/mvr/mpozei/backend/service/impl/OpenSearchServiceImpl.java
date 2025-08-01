package bg.bulsi.mvr.mpozei.backend.service.impl;

import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.mpozei.backend.mapper.HelpPageMapper;
import bg.bulsi.mvr.mpozei.backend.service.OpenSearchService;
import bg.bulsi.mvr.mpozei.contract.dto.FindHelpPagesFilter;
import bg.bulsi.mvr.mpozei.contract.dto.HelpPageDTO;
import bg.bulsi.mvr.mpozei.model.opensearch.HtmlHelpPage;
import bg.bulsi.mvr.mpozei.model.repository.HtmlHelpPageRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.data.domain.Page;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.stream.StreamSupport;

import static bg.bulsi.mvr.common.exception.ErrorCode.*;
import static bg.bulsi.mvr.common.util.ValidationUtil.assertNotBlank;

@Service
@Slf4j
@RequiredArgsConstructor
public class OpenSearchServiceImpl implements OpenSearchService {
    private final HtmlHelpPageRepository htmlHelpPageRepository;
    private final HelpPageMapper helpPageMapper;

    @Override
    public HtmlHelpPage createHelpPage(HtmlHelpPage page) {
        assertNotBlank(page.getPageName(), HELP_PAGE_PAGE_NAME_CANNOT_BE_NULL);
        if (htmlHelpPageRepository.existsByPageNameAndLanguage(page.getPageName(), page.getLanguage())) {
            throw new ValidationMVRException(HELP_PAGE_EXISTS_BY_PAGE_NAME);
        }
        return htmlHelpPageRepository.save(page);
    }

    @Override
    public HtmlHelpPage getHelpPageById(String id) {
        return htmlHelpPageRepository.findById(id)
                .orElseThrow(() -> new EntityNotFoundException(HELP_PAGE_NOT_FOUND_BY_ID, id));
    }

    @Override
    public List<HtmlHelpPage> getHelpPagesByPageName(String pageName) {
        return htmlHelpPageRepository.findByPageName(pageName);
    }

    @Override
    public HtmlHelpPage updateHelpPage(HelpPageDTO dto) {
        assertNotBlank(dto.getPageName(), HELP_PAGE_PAGE_NAME_CANNOT_BE_NULL);
        HtmlHelpPage helpPage = getHelpPagesByPageName(dto.getPageName()).stream().filter(e -> e.getLanguage().equals(dto.getLanguage())).findFirst()
                .orElseThrow(() -> new EntityNotFoundException(HELP_PAGE_NOT_FOUND_BY_PAGE_NAME, dto.getPageName()));
        helpPageMapper.map(helpPage, dto);
        return htmlHelpPageRepository.save(helpPage);
    }

    @Override
    public Page<HtmlHelpPage> findHelpPagesByFilter(FindHelpPagesFilter filter) {
        return htmlHelpPageRepository.findAllByTitleOrContentOrMetaTags(filter.getKeyword(), filter.getLanguage(), filter.getPageable());
    }

    @Override
    public void deleteHelpPageById(String id) {
        HtmlHelpPage entity = getHelpPageById(id);
        htmlHelpPageRepository.delete(entity);
    }

    @Override
    public List<HtmlHelpPage> getAllHelpPages() {
        return StreamSupport.stream(htmlHelpPageRepository.findAll().spliterator(), false).toList();
    }
}

package bg.bulsi.mvr.mpozei.backend.mapper;

import bg.bulsi.mvr.mpozei.contract.dto.HelpPageDTO;
import bg.bulsi.mvr.mpozei.model.opensearch.HtmlHelpPage;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.MappingTarget;
import org.mapstruct.NullValueCheckStrategy;
import org.springframework.data.domain.Page;

import java.util.List;

@Mapper(componentModel = "spring",
        nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
public abstract class HelpPageMapper {

    public abstract HtmlHelpPage map(HelpPageDTO dto);

    public abstract HelpPageDTO map(HtmlHelpPage entity);
    public Page<HelpPageDTO> mapAll(Page<HtmlHelpPage> entities) {
        return entities.map(this::map);
    }
    public List<HelpPageDTO> mapAll(List<HtmlHelpPage> entities) {
        return entities.stream().map(this::map).toList();
    }

    @Mapping(target = "id", ignore = true)
    @Mapping(target = "pageName", ignore = true)
    @Mapping(target = "language", ignore = true)
    public abstract void map(@MappingTarget HtmlHelpPage helpPage, HelpPageDTO dto);
}

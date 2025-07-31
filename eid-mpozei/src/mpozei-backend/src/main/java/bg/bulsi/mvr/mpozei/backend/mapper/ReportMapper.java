package bg.bulsi.mvr.mpozei.backend.mapper;

import bg.bulsi.mvr.mpozei.contract.dto.ApplicationReportByOperatorsDTO;
import bg.bulsi.mvr.mpozei.model.repository.view.ApplicationReportByOperators;
import org.mapstruct.Mapper;
import org.mapstruct.NullValueCheckStrategy;

import java.util.List;

@Mapper(componentModel = "spring",
        nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
public abstract class ReportMapper {

    abstract ApplicationReportByOperatorsDTO maptoDto(ApplicationReportByOperators entity);

    public abstract List<ApplicationReportByOperatorsDTO> mapToDtoList(List<ApplicationReportByOperators> entities);
}
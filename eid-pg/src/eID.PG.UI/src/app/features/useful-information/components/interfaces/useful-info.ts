import { Language } from 'src/app/features/search-content/enum/search';

export interface IHelpPage {
    id: string;
    pageName: string;
    title: string;
    language: Language;
    contentWithHtml: string;
}

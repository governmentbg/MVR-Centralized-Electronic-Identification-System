import { Language } from '../../search/enums/search-enums';

export interface EditorHtmlTarget {
    contentDocument: {
        documentElement: any;
        body: any;
    };
}

export interface IHelpPage {
    id: string;
    pageName: string;
    title: string;
    language: Language;
    contentWithHtml: string;
}

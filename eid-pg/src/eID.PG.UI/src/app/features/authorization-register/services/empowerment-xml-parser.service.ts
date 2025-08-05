import { Injectable } from '@angular/core';
import { IEmpowermentXMLRepresentation } from '../interfaces/authorization-register.interfaces';
import { OnBehalfOf } from '../enums/authorization-register.enum';

@Injectable({
    providedIn: 'root',
})
export class EmpowermentXmlParserService {
    private getChildElementValue(node: any, tagName: any): string {
        const element = node.getElementsByTagName(tagName)[0];
        return element ? element.textContent : null;
    }

    private getNestedChildElementsValues(node: any, parentTagName: any, childTagName: any) {
        const parentElement = node.getElementsByTagName(parentTagName)[0];
        if (!parentElement) {
            return [];
        }

        const elements = node.querySelectorAll(`${parentTagName} > ${childTagName}`);
        const outerResult = Array.from(elements).map((element: any) => {
            const result: any = {};
            Array.from(element.children).forEach((children: any) => {
                const firstLetterLowercaseKey = children.nodeName.charAt(0).toLowerCase() + children.nodeName.slice(1);
                result[firstLetterLowercaseKey] = children.textContent;
            });
            return result;
        });
        return outerResult;
    }

    public convertXmlToObject(xmlString: string): IEmpowermentXMLRepresentation {
        const parser = new DOMParser();
        const xmlDoc = parser.parseFromString(xmlString, 'text/xml');
        const rootNode = xmlDoc.getElementsByTagName('EmpowermentStatementItem')[0];
        return {
            id: this.getChildElementValue(rootNode, 'Id'),
            onBehalfOf: this.getChildElementValue(rootNode, 'OnBehalfOf') as OnBehalfOf,
            uid: this.getChildElementValue(rootNode, 'Uid'),
            name: this.getChildElementValue(rootNode, 'Name'),
            authorizerUids: this.getNestedChildElementsValues(rootNode, 'AuthorizerUids', 'Uid'),
            empoweredUids: this.getNestedChildElementsValues(rootNode, 'EmpoweredUids', 'Uid'),
            providerId: this.getChildElementValue(rootNode, 'ProviderId'),
            providerName: this.getChildElementValue(rootNode, 'ProviderName'),
            serviceId: parseInt(this.getChildElementValue(rootNode, 'ServiceId'), 10),
            serviceName: this.getChildElementValue(rootNode, 'ServiceName'),
            typeOfEmpowerment: this.getChildElementValue(rootNode, 'TypeOfEmpowerment'),
            volumeOfRepresentation: Array.from(rootNode.getElementsByTagName('Item')).map(itemNode => ({
                name: this.getChildElementValue(itemNode, 'Name'),
            })),
            createdOn: this.getChildElementValue(rootNode, 'CreatedOn'),
            startDate: this.getChildElementValue(rootNode, 'StartDate'),
            expiryDate: this.getChildElementValue(rootNode, 'ExpiryDate'),
            number: this.getChildElementValue(rootNode, 'Number'),
        };
    }
}

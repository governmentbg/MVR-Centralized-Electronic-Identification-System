import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root',
})
export class EmpowermentXmlParserService {
    private getChildElementValue(node: any, tagName: any) {
        const element = node.getElementsByTagName(tagName)[0];
        return element ? element.textContent : null;
    }

    private getNestedChildElementsValues(node: any, parentTagName: any, childTagName: any) {
        const parentElement = node.getElementsByTagName(parentTagName)[0];
        if (!parentElement) {
            return [];
        }

        const elements = parentElement.getElementsByTagName(childTagName);
        return Array.from(elements).map((element: any) => {
            return { uid: element.textContent };
        });
    }

    public convertXmlToObject(xmlString: string) {
        const parser = new DOMParser();
        const xmlDoc = parser.parseFromString(xmlString, 'text/xml');
        const rootNode = xmlDoc.getElementsByTagName('EmpowermentStatementItem')[0];

        return {
            id: this.getChildElementValue(rootNode, 'Id'),
            onBehalfOf: this.getChildElementValue(rootNode, 'OnBehalfOf'),
            uid: this.getChildElementValue(rootNode, 'Uid'),
            name: this.getChildElementValue(rootNode, 'Name'),
            authorizerUids: this.getNestedChildElementsValues(rootNode, 'AuthorizerUids', 'Uid'),
            empoweredUids: this.getNestedChildElementsValues(rootNode, 'EmpoweredUids', 'Uid'),
            supplierId: this.getChildElementValue(rootNode, 'SupplierId'),
            supplierName: this.getChildElementValue(rootNode, 'SupplierName'),
            serviceId: this.getChildElementValue(rootNode, 'ServiceId'),
            serviceName: this.getChildElementValue(rootNode, 'ServiceName'),
            typeOfEmpowerment: this.getChildElementValue(rootNode, 'TypeOfEmpowerment'),
            volumeOfRepresentation: Array.from(rootNode.getElementsByTagName('Item')).map(itemNode => ({
                id: this.getChildElementValue(itemNode, 'Code'),
                name: this.getChildElementValue(itemNode, 'Name'),
            })),
            createdOn: this.getChildElementValue(rootNode, 'CreatedOn'),
            startDate: this.getChildElementValue(rootNode, 'StartDate'),
            expiryDate: this.getChildElementValue(rootNode, 'ExpiryDate'),
        };
    }
}

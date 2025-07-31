//
//  StringProtocol+Extension.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 2.11.23.
//

import Foundation


extension StringProtocol {
    public func caseInsensitiveContains<T>(_ other: T) -> Bool where T : StringProtocol {
        return self.lowercased().contains(other.lowercased())
    }
}

extension StringProtocol {
    var attributedOptions: [NSAttributedString.DocumentReadingOptionKey: Any] {
        if #available(iOS 18.0, *) {
          return [
            .documentType: NSAttributedString.DocumentType.html,
            .characterEncoding: String.Encoding.utf8.rawValue,
            .textKit1ListMarkerFormatDocumentOption: true
          ]
        } else {
          return [
            .documentType: NSAttributedString.DocumentType.html,
            .characterEncoding: String.Encoding.utf8.rawValue,
          ]
        }
    }
    
    func htmlToAttributedString() throws -> AttributedString {
        try .init(
            .init(
                data: .init(utf8),
                options: attributedOptions,
                documentAttributes: nil
            )
        )
    }
}

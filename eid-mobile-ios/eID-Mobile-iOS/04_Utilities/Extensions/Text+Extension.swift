//
//  Text+Extension.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 9.10.24.
//

import SwiftUI


extension Text {
    init(html: String, alert: String? = nil) {
        do {
            try self.init(html.htmlToAttributedString())
        } catch {
            self.init(alert ?? error.localizedDescription)
        }
    }
}

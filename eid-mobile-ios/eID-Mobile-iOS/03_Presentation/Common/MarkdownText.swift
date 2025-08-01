//
//  MarkdownText.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 4.06.25.
//

import SwiftUI


struct MarkdownText: View {
    // MARK: - Properties
    let parts: [Text]
    
    init(_ markdown: String) {
        self.parts = MarkdownText.parse(markdown)
    }
    
    // MARK: - Body
    var body: some View {
        parts.reduce(Text(""), +)
    }
    
    private static func parse(_ markdown: String) -> [Text] {
        var result: [Text] = []
        var remaining = markdown
        
        // Patterns and their styling rules
        let patterns: [(regex: NSRegularExpression, style: (String) -> Text)] = [
            (try! NSRegularExpression(pattern: "\\*\\*(.*?)\\*\\*"), { Text($0).font(.bodyBold) }),
            (try! NSRegularExpression(pattern: "__(.*?)__"), { Text($0).font(.bodyRegular).underline() }),
            (try! NSRegularExpression(pattern: "_(.*?)_"), { Text($0).font(.bodyRegular).italic() }),
            (try! NSRegularExpression(pattern: "~~(.*?)~~"), { Text($0).font(.bodyRegular).strikethrough() }),
            (try! NSRegularExpression(pattern: "==(.+?)=="), { Text($0).font(.bodyRegular).foregroundColor(.yellow) }),
            
            // Custom color: {#ff0000|Red text}
            (try! NSRegularExpression(pattern: "\\{#([0-9a-fA-F]{6})\\|(.*?)\\}"), { hexText in
                let components = hexText.components(separatedBy: "|")
                if components.count == 2,
                   let color = Color(hex: components[0]) {
                    return Text(components[1]).font(.bodyRegular).foregroundColor(color)
                }
                return Text(hexText)
            }),
            
            // Custom font size: {font:20|Big Text}
            (try! NSRegularExpression(pattern: "\\{font:(\\d+)\\|(.*?)\\}"), { sizeText in
                let components = sizeText.components(separatedBy: "|")
                if components.count == 2,
                   let sizePart = components.first,
                   let size = Double(sizePart.components(separatedBy: ":").last ?? "") {
                    return Text(components[1]).font(.system(size: size))
                }
                return Text(sizeText)
            })
        ]
        
        while !remaining.isEmpty {
            var matched = false
            
            for (regex, styleBuilder) in patterns {
                if let match = regex.firstMatch(in: remaining, range: NSRange(location: 0, length: remaining.utf16.count)) {
                    let fullRange = Range(match.range, in: remaining)!
                    let before = String(remaining[..<fullRange.lowerBound])
                    let groups = (1..<match.numberOfRanges).compactMap {
                        Range(match.range(at: $0), in: remaining).map { String(remaining[$0]) }
                    }
                    let after = String(remaining[fullRange.upperBound...])
                    
                    if !before.isEmpty {
                        result.append(Text(before))
                    }
                    
                    if groups.isEmpty {
                        result.append(Text(String(remaining[fullRange])))
                    } else {
                        result.append(styleBuilder(groups.joined(separator: "|")))
                    }
                    
                    remaining = after
                    matched = true
                    break
                }
            }
            
            if !matched {
                result.append(Text(remaining))
                break
            }
        }
        
        return result
    }
}


struct MarkdownRenderer: View {
    let blocks: [MarkdownBlock]
    
    init(_ markdown: String) {
        self.blocks = MarkdownRenderer.parse(markdown)
    }
    
    var body: some View {
        VStack(alignment: .leading, spacing: 16) {
            ForEach(blocks.indices, id: \.self) { index in
                switch blocks[index] {
                case .paragraph(let text):
                    MarkdownText(text)
                case .list(let items):
                    VStack(alignment: .leading, spacing: 8) {
                        ForEach(items.indices, id: \.self) { i in
                            HStack(alignment: .top, spacing: 8) {
                                Text("•")
                                MarkdownText(items[i])
                            }
                        }
                    }
                }
            }
        }
    }
    
    enum MarkdownBlock {
        case paragraph(String)
        case list([String])
    }
    
    private static func parse(_ markdown: String) -> [MarkdownBlock] {
        var result: [MarkdownBlock] = []
        let lines = markdown.components(separatedBy: .newlines)
        
        var currentParagraph: [String] = []
        var currentList: [String] = []
        
        func flushParagraph() {
            if !currentParagraph.isEmpty {
                result.append(.paragraph(currentParagraph.joined(separator: " ")))
                currentParagraph.removeAll()
            }
        }
        
        func flushList() {
            if !currentList.isEmpty {
                result.append(.list(currentList))
                currentList.removeAll()
            }
        }
        
        for line in lines {
            let trimmed = line.trimmingCharacters(in: .whitespaces)
            if trimmed.hasPrefix("- ") || trimmed.hasPrefix("* ") {
                flushParagraph()
                currentList.append(String(trimmed.dropFirst(2)))
            } else if trimmed.isEmpty {
                flushParagraph()
                flushList()
            } else {
                flushList()
                currentParagraph.append(trimmed)
            }
        }
        
        flushParagraph()
        flushList()
        
        return result
    }
}

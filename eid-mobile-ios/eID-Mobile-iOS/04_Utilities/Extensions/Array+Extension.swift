//
//  Array+Extension.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 19.10.23.
//

import Foundation


extension Array where Element: Hashable {
    // MARK: - Append without duplicates
    @discardableResult
    mutating func appendIfMissing(_ element: Element) -> (appended: Bool, memberAfterAppend: Element) {
        if let index = firstIndex(of: element) {
            return (false, self[index])
        } else {
            append(element)
            return (true, element)
        }
    }
    
    mutating func appendIfMissing(contentsOf elements: [Element]) {
        for element in elements {
            appendIfMissing(element)
        }
    }
    
    // MARK: - Remove if contains
    @discardableResult
    mutating func removeIfExists(_ element: Element) -> (removed: Bool, count: Int) {
        var removedCount: Int = 0
        while contains(element) {
            if let index = firstIndex(of: element) {
                removedCount += 1
                remove(at: index)
            }
        }
        return (removedCount != 0, removedCount)
    }
    
    mutating func removeIfExists(contentsOf elements: [Element]) {
        for element in elements {
            removeIfExists(element)
        }
    }
}


extension Array where Element == String {
    var isEmptyOrEmptyElements: Bool {
        return self.filter({ !$0.isEmpty && $0 != "" }).isEmpty
    }
}

extension Array where Element == String? {
    var fullName: String {
        var fullName = ""
        for name in self {
            if let name = name, !name.isEmpty {
                fullName.append("\(name) ")
            }
        }
        if fullName.isEmpty == false {
            fullName.removeLast()
        }
        return fullName
    }
}

extension Array where Element: Equatable {
    mutating func move(_ element: Element, to newIndex: Index) {
        if let oldIndex: Int = self.firstIndex(of: element) { self.move(from: oldIndex, to: newIndex) }
    }
}

extension Array {
    mutating func move(from oldIndex: Index, to newIndex: Index) {
        if oldIndex == newIndex { return }
        if abs(newIndex - oldIndex) == 1 { return self.swapAt(oldIndex, newIndex) }
        self.insert(self.remove(at: oldIndex), at: newIndex)
    }
}

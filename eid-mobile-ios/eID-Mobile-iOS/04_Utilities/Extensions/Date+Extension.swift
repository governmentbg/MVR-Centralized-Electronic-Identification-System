//
//  Date+Extension.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 5.11.23.
//

import Foundation


// MARK: - Date Formats
enum DateFormat: String {
    case dayMonthYear = "dd MMM yyyy"
    case monthYear = "MM/yy"
    case fullMonthYear = "MMMM yyyy"
    case dayFullMonthYear = "dd MMMM yyyy"
    case dayFullMonthTime = "dd MMMM HH:mm"
    case dayFullMonth = "dd MMMM"
    case dayHalfMonth = "dd MMM"
    case dayMonthYearTime = "dd MMM yyyy HH:mm"
    case iso8601NoFractions = "yyyy-MM-dd'T'HH:mm:ss"
    case iso8601NoFractionsUTC = "yyyy-MM-dd'T'HH:mm:ss'Z'"
    case iso8601 = "yyyy-MM-dd'T'HH:mm:ss.SSS"
    case iso8601UTC = "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'"
    case iso8601Date = "yyyy-MM-dd"
    case iso8601DateTime = "yyyy-MM-dd, HH:mm:ss"
    case compactFullYear = "dd.MM.yyyy"
    case compactShortYear = "dd.MM.yy"
    case time = "HH:mm"
    case fullTime = "HH:mm:ss"
    case day = "dd"
    case dayFullTime = "yyyy-MM-dd HH:mm"
    case log = "yyyy-MM-dd-HH-mm-ss"
}


// MARK: - Calendar
extension Calendar {
    public static var generic: Calendar {
        var cal = Calendar.current
        cal.timeZone = .gmt
        return cal
    }
}


// MARK: - TimeZone
extension TimeZone {
    public static var generic: TimeZone {
        return TimeZone(secondsFromGMT: 0)!
    }
}

// MARK: - Init with Milliseconds
extension Date {
    var millisecondsSince1970: UInt64 {
        return UInt64((self.timeIntervalSince1970 * 1000.0).rounded())
    }
    
    init(milliseconds: UInt64) {
        self = Date(timeIntervalSince1970: TimeInterval(milliseconds) / 1000)
    }
}


// MARK: - String to Date
extension DateFormatter {
    convenience init (format: String) {
        self.init()
        timeZone = .generic
        dateFormat = format
    }
}


// MARK: - Get date components
extension Date {
    var day: Int { return Calendar.current.component(.day, from: self) }
    var month: Int { return Calendar.current.component(.month, from: self) }
    var year: Int { return Calendar.current.component(.year, from: self) }
    var hours: Int { return Calendar.current.component(.hour, from: self) }
    var minutes: Int { return Calendar.current.component(.minute, from: self) }
    
    func getTimeString() -> String {
        let formatter = DateFormatter()
        formatter.dateFormat = DateFormat.time.rawValue
        return formatter.string(from: self)
    }
}


// MARK: - TimeZone
extension Date {
    func toGlobalTime() -> Date {
        let timezone = TimeZone.current
        let seconds = -TimeInterval(timezone.secondsFromGMT(for: self))
        return Date(timeInterval: seconds, since: self)
    }
    
    func toLocalTime() -> Date {
        let seconds = TimeInterval(TimeZone.current.secondsFromGMT(for: self))
        return Date(timeInterval: seconds, since: self)
    }
}


// MARK: - Date modifications
extension Date {
    var startOfDay: Date {
        return Calendar.generic.date(bySettingHour: 0, minute: 0, second: 0, of: self)!
    }
    
    var endOfDay: Date {
        return Calendar.generic.date(bySettingHour: 23, minute: 59, second: 59, of: self)!.addingTimeInterval(0.999)
    }
    
    var startOfMonth: Date {
        let calendar = Calendar(identifier: .gregorian)
        let components = calendar.dateComponents([.year, .month], from: self)
        return  calendar.date(from: components)!.toLocalTime()
        /// Local time, DO NOT CONVERT when sending for filter
    }

    var endOfMonth: Date {
        let calendar = Calendar(identifier: .gregorian)
        var components = DateComponents()
        components.month = 1
        components.second = -1
        return calendar.date(byAdding: components, to: startOfMonth)!
        /// Local time, DO NOT CONVERT when sending for filter
    }
    
    func normalizeDate(outputFormat: DateFormat) -> String {
        return DateFormatter(format: outputFormat.rawValue).string(from: self)
    }
    
    func addSecond(n: Int) -> Date {
        return Calendar.generic.date(byAdding: .second, value: n, to: self)!
    }
    
    func addMinute(n: Int) -> Date {
        return Calendar.generic.date(byAdding: .minute, value: n, to: self)!
    }
    
    func addHour(n: Int) -> Date {
        return Calendar.generic.date(byAdding: .hour, value: n, to: self)!
    }
    
    func addDay(n: Int) -> Date {
        return Calendar.generic.date(byAdding: .day, value: n, to: self)!
    }
    
    func addMonth(n: Int) -> Date {
        return Calendar.generic.date(byAdding: .month, value: n, to: self)!
    }
    
    func addYear(n: Int) -> Date {
        return Calendar.generic.date(byAdding: .year, value: n, to: self)!
    }
}

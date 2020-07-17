
import moment from 'moment'

function parse_utc_time(time) {
    return time.add(-8, 'hour').format('YYYY-MM-DD HH:mm:ss');
}

function parse_date_range(time_range) {

    var p = {
        start_time_utc: null,
        end_time_utc: null
    };

    if (time_range && time_range.length === 2) {
        var start_moment = time_range[0];
        var end_moment = time_range[1];
        if (start_moment && end_moment) {
            p.start_time_utc = parse_utc_time(moment(start_moment.format('YYYY-MM-DD')));
            p.end_time_utc = parse_utc_time(moment(end_moment.format('YYYY-MM-DD')).add(1, 'day'));
        }
    }

    return p;
}

function parse_month_range(time_range) {

    var p = {
        start_time_utc: null,
        end_time_utc: null
    };

    if (time_range && time_range.length === 2) {
        var start_moment = time_range[0];
        var end_moment = time_range[1];
        if (start_moment && end_moment) {
            p.start_time_utc = parse_utc_time(moment(start_moment.format('YYYY-MM')));
            p.end_time_utc = parse_utc_time(moment(end_moment.format('YYYY-MM')).add(1, 'month'));
        }
    }

    return p;
}

function parse_pagination(pagination, res) {
    pagination.pageSize = res.Data.PageSize
    pagination.total = res.Data.ItemCount
    return pagination;
}

export { parse_utc_time, parse_date_range, parse_month_range, parse_pagination }
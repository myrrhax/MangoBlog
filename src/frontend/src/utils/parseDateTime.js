export default function parseDateTime(datetime) {
    const split = datetime.split('T');

    const date = split[0].replaceAll('-', '.');
    const time = split[1].replace(/\.\d+.*$/, '');

    return date + ' ' + time;
}
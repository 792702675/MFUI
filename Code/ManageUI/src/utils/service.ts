export async function getLessjs() {
    return fetch('/color/less.min.js', {
        method: 'get',
        withCredentials: true,
        credentials: 'include'
    }).then((response) => response.text());
}
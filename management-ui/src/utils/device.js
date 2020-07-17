import enquireJs from 'enquire.js'

export const DEVICE_TYPE = {
  DESKTOP: 'desktop',
  TABLET: 'tablet',
  MOBILE: 'mobile'
}

export const deviceEnquire = function (callback) {
  // screen and (max-width: 1087.99px)
  enquireJs
    .register('screen and (max-width: 576px)', {
      match: () => {
        callback && callback(DEVICE_TYPE.MOBILE)
      }
    })
    .register('screen and (min-width: 576px) and (max-width: 1199px)', {
      match: () => {
        callback && callback(DEVICE_TYPE.TABLET)
      }
    })
    .register('screen and (min-width: 1200px)', {
      match: () => {
        callback && callback(DEVICE_TYPE.DESKTOP)
      }
    })
}

const getters = {
  //app
  device: state => state.app.device,
  theme: state => state.app.theme,
  color: state => state.app.color,
  //user
  avatar: state => state.user.UserImg,
  nickname: state => state.user.NickName,
  userInfo: state => state.user.info,
}

export default getters

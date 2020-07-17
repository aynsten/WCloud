
const app = {
  state: {
    sidebar: true,
    device: 'desktop',
  },
  mutations: {
    SET_SIDEBAR_TYPE: (state, type) => {
      state.sidebar = type
    },
    CLOSE_SIDEBAR: (state) => {
      state.sidebar = false
    },
    TOGGLE_DEVICE: (state, device) => {
      state.device = device
    },
  },
  actions: {
  }
}

export default app

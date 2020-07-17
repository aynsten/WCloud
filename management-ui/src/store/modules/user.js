import { post_form } from '@/api/ajax'

const user = {
  state: {
    info: {},
    permissions: []
  },

  mutations: {
    SET_INFO: (state, data) => {
      state.info = data || {}
    },
    GET_INFO: (state, vue) => {
      post_form(vue, '/api/member/admin/Account/GetLoginAdminInfo', {}).then(res => {
        state.info = res.Data || {}
      });
      post_form(vue, '/api/member/admin/Permission/MyPermissions', {}).then(res => {
        state.permissions = res.Data || [];
      });
    }
  },

  actions: {
    // 获取用户信息
    GetInfo({ commit }) {
    },
  }
}

export default user

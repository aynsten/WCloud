<template>
  <div class="user-wrapper">
    <div class="content-box">
      <a v-if="false" href="https://pro.loacg.com/docs/getting-started" target="_blank">
        <span class="action">
          <a-icon type="question-circle-o"></a-icon>
        </span>
      </a>
      <notice-icon v-if="false" class="action" />
      <a-dropdown>
        <span class="action ant-dropdown-link user-dropdown-menu">
          <a-avatar class="avatar" size="small" :src="login_user.UserImg||'/admin/logo.png'" />
          <span>{{ login_user.NickName||login_user.UserName||'User' }}</span>
        </span>
        <a-menu slot="overlay">
          <a-menu-item key="0" disabled>
            <router-link :to="{ name: 'account-settings' }">
              <a-icon type="user" />
              <span>个人中心</span>
            </router-link>
          </a-menu-item>
          <a-menu-item key="1">
            <router-link :to="{ name: 'account-settings' }">
              <a-icon type="setting" />
              <span>账户设置</span>
            </router-link>
          </a-menu-item>
          <a-menu-divider />
          <a-menu-item key="3">
            <a href="javascript:;" @click="handleLogout">
              <a-icon type="logout" />
              <span>退出登录</span>
            </a>
          </a-menu-item>
        </a-menu>
      </a-dropdown>
    </div>
  </div>
</template>

<script>
import NoticeIcon from '@/components/NoticeIcon'
import { post_form } from '@/api/ajax'

export default {
  components: {
    NoticeIcon
  },
  data() {
    return {
      login_user: {}
    }
  },
  watch: {
    '$store.state.user.info': function(val) {
      this.set_login_user(val)
    }
  },
  mounted() {
    this.set_login_user(this.$store.state.user.info)
  },
  methods: {
    set_login_user(val) {
      this.login_user = val || {}
    },
    handleLogout() {
      if (!confirm('确定退出登录？')) {
        return
      }
      const that = this
      post_form(this, '/api/member/admin/Account/LogOutAction', {}).then(res => {
        if (res.Success) {
          this.$store.commit('GET_INFO', this)
          this.$router.push({ name: 'login' })
        } else {
          alert(res.ErrorMsg)
        }
      })
    }
  }
}
</script>

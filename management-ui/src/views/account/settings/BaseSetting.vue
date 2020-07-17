<template>
  <div class="account-settings-info-view">
    <a-spin :spinning="loading">
      <a-row :gutter="16">
        <a-col :md="24" :lg="16">
          <a-form layout="vertical">
            <a-form-item label="用户名">
              <a-input v-model="data.UserName" disabled />
            </a-form-item>
            <a-form-item label="昵称">
              <a-input v-model="data.NickName" />
            </a-form-item>
            <a-form-item label="电话号码" :required="false">
              <a-input placeholder="139-xxx" v-model="data.UserPhone" />
            </a-form-item>
            <a-form-item label="电子邮件" :required="false">
              <a-input placeholder="exp@admin.com" v-model="data.UserEmail" />
            </a-form-item>
            <a-form-item label="性别">
              <a-select v-model="data.Sex" defaultValue="-1">
                <a-select-option :value="-1">未知</a-select-option>
                <a-select-option :value="1">男</a-select-option>
                <a-select-option :value="0">女</a-select-option>
              </a-select>
            </a-form-item>

            <a-form-item>
              <a-button type="primary" size="small" @click="save_profile" :loading="saving">保存</a-button>
            </a-form-item>
          </a-form>
        </a-col>
        <a-col :md="24" :lg="8" :style="{ minHeight: '180px' }">
          <div class="ant-upload-preview" @click="$refs.modal.edit(1)">
            <div class="mask">
              <a-icon type="plus" />
            </div>
            <img :src="data.UserImg" />
          </div>
        </a-col>
      </a-row>

      <avatar-modal ref="modal"></avatar-modal>
    </a-spin>
  </div>
</template>

<script>
import AvatarModal from './AvatarModal'
import { post_form } from '@/api/ajax'

export default {
  components: {
    AvatarModal
  },
  data() {
    return {
      loading: false,
      saving: false,
      data: {
        UID: '',
        UserName: '',
        NickName: '',
        UserPhone: '',
        UserImg: '',
        UserEmail: ''
      }
    }
  },
  methods: {
    get_profile() {
      this.loading = true
      post_form(this, '/api/member/admin/User/Profile', {})
        .then(res => {
          if (res.Success) {
            this.data = res.Data
          }
        })
        .finally(() => {
          this.loading = false
        })
    },
    save_profile() {
      this.saving = true
      post_form(this, '/api/member/admin/User/UpdateProfile', {
        data: JSON.stringify(this.data)
      })
        .then(res => {
          if (res.Success) {
            alert('保存成功')
            this.$store.commit('GET_INFO')
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          this.saving = false
        })
    }
  },
  created() {
    this.get_profile()
  }
}
</script>

<style lang="less" scoped>
.avatar-upload-wrapper {
  height: 200px;
  width: 100%;
}

.ant-upload-preview {
  position: relative;
  margin: 0 auto;
  width: 100%;
  max-width: 180px;
  border-radius: 50%;
  box-shadow: 0 0 4px #ccc;

  .upload-icon {
    position: absolute;
    top: 0;
    right: 10px;
    font-size: 1.4rem;
    padding: 0.5rem;
    background: rgba(222, 221, 221, 0.7);
    border-radius: 50%;
    border: 1px solid rgba(0, 0, 0, 0.2);
  }
  .mask {
    opacity: 0;
    position: absolute;
    background: rgba(0, 0, 0, 0.4);
    cursor: pointer;
    transition: opacity 0.4s;

    &:hover {
      opacity: 1;
    }

    i {
      font-size: 2rem;
      position: absolute;
      top: 50%;
      left: 50%;
      margin-left: -1rem;
      margin-top: -1rem;
      color: #d6d6d6;
    }
  }

  img,
  .mask {
    width: 100%;
    max-width: 180px;
    height: 100%;
    border-radius: 50%;
    overflow: hidden;
  }
}
</style>

using AutoMapper;
using InfertilityTreatment.Business.Helpers;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Users;
using InfertilityTreatment.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<string> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var password = PasswordHelper.HashPassword(changePasswordDto.Password);

            var result = await _userRepository.ChangePasswordAsync(userId, password);
            if (result == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }
            return "Profile updated successfully";

        }

        public async Task<UserProfileDto> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdWithProfilesAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }
            var proileDto = _mapper.Map<UserProfileDto>(user);
            return proileDto;
        }

        public async Task<string> UpdateProfileAsync(int userId, UpdateProfileDto updateProfileDto)
        {
            var user = _mapper.Map<User>(updateProfileDto);
            user.Id = userId;

            var result = await _userRepository.UpdateProfile(user);
            if (result == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }
            return "Profile updated successfully";

        }
    }

}
